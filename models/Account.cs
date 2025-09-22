using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LS.models;

abstract class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    [Required]
    [StringLength(100)]
    public required string Email { get; set; }
    [Required]
    [StringLength(100)]
    public required string PasswordHash { get; set; }
}