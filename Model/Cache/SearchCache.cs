using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;

namespace Es.Udc.DotNet.PracticaMAD.Model.Cache
{
    /// <summary>
    /// Implementacion de <see cref="ISearchCache"/> basada en <see cref="MemoryCache"/>
    /// (<c>System.Runtime.Caching</c>) que mantiene en memoria las N ultimas consultas
    /// realizadas por los usuarios de la aplicacion (por defecto, las 5 ultimas).
    /// </summary>
    public class SearchCache : ISearchCache
    {
        public const int DefaultMaxEntries = 5;

        private readonly int maxEntries;
        private readonly MemoryCache memoryCache;
        private readonly LinkedList<string> recentKeys;
        private readonly Dictionary<string, LinkedListNode<string>> keyNodes;
        private readonly object syncRoot = new object();
        private bool disposed;

        public SearchCache()
            : this(DefaultMaxEntries)
        {
        }

        public SearchCache(int maxEntries)
        {
            if (maxEntries <= 0)
            {
                throw new ArgumentOutOfRangeException("maxEntries");
            }

            this.maxEntries = maxEntries;
            this.memoryCache = new MemoryCache("PracticaMAD_SearchCache_" + Guid.NewGuid().ToString("N"));
            this.recentKeys = new LinkedList<string>();
            this.keyNodes = new Dictionary<string, LinkedListNode<string>>(StringComparer.Ordinal);
        }

        public int MaxEntries
        {
            get { return maxEntries; }
        }

        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    return keyNodes.Count;
                }
            }
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            lock (syncRoot)
            {
                ThrowIfDisposed();

                LinkedListNode<string> node;
                if (!keyNodes.TryGetValue(key, out node))
                {
                    value = null;
                    return false;
                }

                T cached = memoryCache.Get(key) as T;
                if (cached == null)
                {
                    recentKeys.Remove(node);
                    keyNodes.Remove(key);
                    value = null;
                    return false;
                }

                // Al repetirse la consulta pasa a ser la mas reciente de las ultimas realizadas
                if (recentKeys.First != node)
                {
                    recentKeys.Remove(node);
                    recentKeys.AddFirst(node);
                }

                value = CloneIfSupported(cached);
                return true;
            }
        }

        public void Put<T>(string key, T value) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            lock (syncRoot)
            {
                ThrowIfDisposed();

                T copyToStore = CloneIfSupported(value);
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                    Priority = CacheItemPriority.NotRemovable
                };

                LinkedListNode<string> existingNode;
                if (keyNodes.TryGetValue(key, out existingNode))
                {
                    if (recentKeys.First != existingNode)
                    {
                        recentKeys.Remove(existingNode);
                        recentKeys.AddFirst(existingNode);
                    }

                    memoryCache.Set(key, copyToStore, policy);
                    return;
                }

                if (keyNodes.Count >= maxEntries && recentKeys.Last != null)
                {
                    string oldestKey = recentKeys.Last.Value;
                    recentKeys.RemoveLast();
                    keyNodes.Remove(oldestKey);
                    memoryCache.Remove(oldestKey);
                }

                LinkedListNode<string> newNode = recentKeys.AddFirst(key);
                keyNodes[key] = newNode;
                memoryCache.Set(key, copyToStore, policy);
            }
        }

        public bool Contains(string key)
        {
            if (key == null)
            {
                return false;
            }

            lock (syncRoot)
            {
                ThrowIfDisposed();
                return keyNodes.ContainsKey(key) && memoryCache.Contains(key);
            }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                ThrowIfDisposed();

                foreach (string key in recentKeys)
                {
                    memoryCache.Remove(key);
                }

                recentKeys.Clear();
                keyNodes.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (syncRoot)
                {
                    if (!disposed)
                    {
                        recentKeys.Clear();
                        keyNodes.Clear();
                        memoryCache.Dispose();
                        disposed = true;
                    }
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(typeof(SearchCache).FullName);
            }
        }

        private static T CloneIfSupported<T>(T instance) where T : class
        {
            Block<ProductVO> productBlock = instance as Block<ProductVO>;
            if (productBlock != null)
            {
                List<ProductVO> clonedItems = productBlock.Items != null
                    ? new List<ProductVO>(productBlock.Items)
                    : new List<ProductVO>();

                return new Block<ProductVO>(clonedItems, productBlock.ExistsMoreItems) as T;
            }

            return instance;
        }
    }
}

