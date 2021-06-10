
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace IdentityServices.Models
{
	[CollectionName ("Users")]
    public class ApplicationUser : MongoIdentityUser
	{
        public string FullName { get; set; }
    }
}
