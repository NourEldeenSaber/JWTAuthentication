namespace Domain.Entities.IdentitiyModule
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        #region Address [1] => User[1] : Mandatory
        public ApplicationUser User { get; set; } = default!;
        public string UserId { get; set; } = default!;

        #endregion
    }
}