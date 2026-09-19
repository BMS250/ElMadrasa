using ManageData.Models;
using Microsoft.AspNetCore.Identity;
using MyProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class ServantClass
{
    public string Id { get; set; }
    public string ServantId { get; set; }

    [ForeignKey("ServantId")]
    public IdentityUser Servant { get; set; }

    public string ClassId { get; set; }

    [ForeignKey("ClassId")]
    public Class Class { get; set; }

    public Subject Subject { get; set; }
}