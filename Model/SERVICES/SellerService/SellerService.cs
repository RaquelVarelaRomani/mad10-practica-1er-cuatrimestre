using System;
using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Ninject;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class SellerService : ISellerService
    {
        private const int MinScore = 1;
        private const int MaxScore = 5;
        private const int MaxCommentLength = 1000;

        [Inject]
        public ISellerRatingDao SellerRatingDao { private get; set; }

        [Inject]
        public IProductDao ProductDao { private get; set; }

        [Inject]
        public IUserProfileDao UserProfileDao { private get; set; }

        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="SelfRatingException"/>
        /// <exception cref="InvalidRatingScoreException"/>
        /// <exception cref="DuplicateInstanceException"/>
        /// <exception cref="ArgumentException"/>
        [Transactional]
        public long RateSeller(long buyerUserId, long productId, int score, string comment = null)
        {
            UserProfileDao.Find(buyerUserId);
            Product product = ProductDao.Find(productId);

            if (product.sellerId == buyerUserId)
            {
                throw new SelfRatingException(buyerUserId);
            }

            if (score < MinScore || score > MaxScore)
            {
                throw new InvalidRatingScoreException(score);
            }

            string normalizedComment = string.IsNullOrWhiteSpace(comment)
                ? null
                : comment.Trim();

            if (normalizedComment != null && normalizedComment.Length > MaxCommentLength)
            {
                throw new ArgumentException(
                    "El comentario justificativo no puede superar los 1000 caracteres.",
                    "comment");
            }

            SellerRating existingRating = SellerRatingDao.FindBySellerAndProductAndBuyer(
                product.sellerId,
                productId,
                buyerUserId);

            if (existingRating != null)
            {
                throw new DuplicateInstanceException(
                    existingRating.ratingId,
                    typeof(SellerRating).FullName);
            }

            SellerRating rating = new SellerRating
            {
                sellerId = product.sellerId,
                productId = productId,
                buyerUserId = buyerUserId,
                score = score,
                comment = normalizedComment,
                ratingDate = DateTime.Now
            };

            SellerRatingDao.Create(rating);

            return rating.ratingId;
        }

        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        public SellerRatingSummaryVO GetSellerRatingSummary(string sellerLoginName)
        {
            UserProfile seller = UserProfileDao.FindByLoginName(sellerLoginName);

            return BuildSummary(seller);
        }

        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        public SellerRatingSummaryVO GetSellerRatingSummary(long sellerId)
        {
            UserProfile seller = UserProfileDao.Find(sellerId);

            return BuildSummary(seller);
        }

        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        public Block<SellerRatingVO> FindSellerRatings(string sellerLoginName, int startIndex, int count)
        {
            UserProfile seller = UserProfileDao.FindByLoginName(sellerLoginName);

            return FindSellerRatingsForUser(seller, startIndex, count);
        }

        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        public Block<SellerRatingVO> FindSellerRatings(long sellerId, int startIndex, int count)
        {
            UserProfile seller = UserProfileDao.Find(sellerId);

            return FindSellerRatingsForUser(seller, startIndex, count);
        }

        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        public SellerRatingVO FindSellerRatingById(long ratingId)
        {
            SellerRating rating = SellerRatingDao.Find(ratingId);

            return ToSellerRatingVO(rating, null);
        }

        private SellerRatingSummaryVO BuildSummary(UserProfile seller)
        {
            int totalRatings = SellerRatingDao.GetNumberOfRatingsBySeller(seller.usrId);
            double averageScore = SellerRatingDao.GetAverageRatingBySeller(seller.usrId);

            return new SellerRatingSummaryVO(
                seller.usrId,
                seller.loginName,
                totalRatings,
                averageScore);
        }

        private Block<SellerRatingVO> FindSellerRatingsForUser(UserProfile seller, int startIndex, int count)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            List<SellerRating> ratings =
                SellerRatingDao.FindBySellerId(seller.usrId, startIndex, count + 1);

            bool existMoreItems = (ratings.Count == count + 1);

            if (existMoreItems)
            {
                ratings.RemoveAt(count);
            }

            List<SellerRatingVO> ratingVOs = new List<SellerRatingVO>(ratings.Count);

            foreach (SellerRating rating in ratings)
            {
                ratingVOs.Add(ToSellerRatingVO(rating, seller.loginName));
            }

            return new Block<SellerRatingVO>(ratingVOs, existMoreItems);
        }

        private SellerRatingVO ToSellerRatingVO(SellerRating rating, string knownSellerLoginName)
        {
            string sellerLoginName = knownSellerLoginName
                ?? (rating.UserProfile1 != null
                    ? rating.UserProfile1.loginName
                    : UserProfileDao.Find(rating.sellerId).loginName);

            string buyerLoginName = rating.UserProfile != null
                ? rating.UserProfile.loginName
                : UserProfileDao.Find(rating.buyerUserId).loginName;

            string productName = rating.Product != null
                ? rating.Product.productName
                : ProductDao.Find(rating.productId).productName;

            return new SellerRatingVO(
                rating.ratingId,
                rating.sellerId,
                sellerLoginName,
                rating.productId,
                productName,
                rating.buyerUserId,
                buyerLoginName,
                rating.score,
                rating.comment,
                rating.ratingDate);
        }
    }
}

