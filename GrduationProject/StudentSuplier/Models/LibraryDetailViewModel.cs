namespace StudentSuplier.Models
{
    public class LibraryDetailViewModel
    {
        public int LibraryId { get; set; }
        public string LibraryName { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public string WorkingHour { get; set; }
        public string Description { get; set; }
        public List<Product>? Products { get; set; }
    }
}
