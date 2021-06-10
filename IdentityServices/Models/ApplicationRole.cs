
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace IdentityServices.Models
{

    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole
    {
	}
}
