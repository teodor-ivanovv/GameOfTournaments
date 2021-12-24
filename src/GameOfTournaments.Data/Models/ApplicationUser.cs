namespace GameOfTournaments.Data.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    /// <inheritdoc />
    public class ApplicationUser : IdentityUser<int>
    {
        /// <summary>
        /// Gets or sets the age of this <see cref="ApplicationUser"/>.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the first name of this <see cref="ApplicationUser"/>.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name of this <see cref="ApplicationUser"/>.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of this <see cref="ApplicationUser"/>.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ApplicationUser"/> is male.
        /// </summary>
        public bool Male { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ApplicationUserAccount"/> navigation property pointing to this <see cref="ApplicationUser"/>.
        /// </summary>
        public ApplicationUserAccount ApplicationUserAccount { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the <see cref="ApplicationUserAccount"/> pointing to this <see cref="ApplicationUser"/>.
        /// </summary>
        public int ApplicationUserAccountId { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="IEnumerable{T}"/> of <see cref="Permission"/> representing the roles of this <see cref="ApplicationUser"/>.
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; } = new HashSet<Permission>();
    }
}