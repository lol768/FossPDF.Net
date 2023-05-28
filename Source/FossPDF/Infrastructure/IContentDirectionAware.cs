namespace FossPDF.Infrastructure
{
    internal interface IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
    }
}
