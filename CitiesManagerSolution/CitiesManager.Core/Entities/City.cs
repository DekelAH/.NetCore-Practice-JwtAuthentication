using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Entities
{
    public class City
    {
        #region Properties

        [Key]
        public Guid CityID { get; set; }

        [Required(ErrorMessage = "CityName can't be blank")]
        public string? CityName { get; set; }


        #endregion
    }
}
