using SK.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ApiWithAuthentication.Domains.Core.Identity.Entities
{
    public class User : IdentityUser<Guid>, IEntity, IAudited<User>, ISoftDelete<User>
    {
        [Required]
        public string Firstname { get; private set; }
        [Required]
        public string Lastname { get; private set; }
        public string ProfileImageUrl { get; private set; }

        private string _fullName;
        public string FullName
        {
            get
            {
                return _fullName ?? $"{Firstname} {Lastname}";
            }
            private set
            {
                _fullName = value;
            }
        }

        [NotMapped]
        public string RoleName
        {
            get
            {
                return UserRoles?.FirstOrDefault()?.Role?.Name;
            }
        }

        public DateTimeOffset? CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedByUserId { get; set; }
        public virtual User DeletedByUser { get; set; }
        public Guid? InvitedByUserId { get; set; }
        public virtual User InvitedByUser { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        protected User() { }

        public User(string email, string firstname, string lastname, bool emailConfirmed = false)
        {
            UserName = email;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            EmailConfirmed = emailConfirmed;
            GenerateNewSecurityStamp();
        }

        public User(string email, string firstname, string lastname, User invitedByUser, bool emailConfirmed = false) 
            : this(email,firstname,lastname,emailConfirmed)
        {
            InvitedByUserId = invitedByUser.Id;
        }

        public User SetProfileImageUrl(string profileImageUrl)
        {
            ProfileImageUrl = profileImageUrl;
            return this;
        }

        public bool Equals(IEntity x, IEntity y)
        {
            return x.Id == y.Id;
        }

        public void GenerateNewSecurityStamp()
        {
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public void Update(string firstname, string lastname)
        {
            Firstname = firstname;
            Lastname = lastname;
        }

        public void SetRole(Role role)
        {
            if (RoleName != role.Name)
            {
                UserRoles.Clear();
                UserRoles.Add(new UserRole()
                {
                    User = this,
                    Role = role
                });
            }
        }
    }
}
