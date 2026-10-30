using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    [Serializable]
    public class TagVO
    {
        public long TagId { get; private set; }
        public string TagName { get; private set; }
        public int Frequency { get; private set; }

        public TagVO(long tagId, string tagName, int frequency = 0)
        {
            this.TagId = tagId;
            this.TagName = tagName;
            this.Frequency = frequency;
        }
    }
}
