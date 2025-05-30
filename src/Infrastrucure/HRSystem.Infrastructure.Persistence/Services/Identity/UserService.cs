﻿using AutoMapper;
using HRSystem.Application.Services;
using HRSystem.Application.Services.Identity;
using HRSystem.Common.Authorization;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Identity;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ICurrentUserService currentUserService,
            IEmailSender emailSender,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest)
        {
          // check if user exist
          var userWithSameEmail = await _userManager.FindByEmailAsync(userRegistrationRequest.Email);
            if (userWithSameEmail is not null)
                return await ResponseWrapper.FailAsync("this email is already taken");

            var userWithSameName = await _userManager.FindByNameAsync(userRegistrationRequest.UserName);
            if (userWithSameName is not null)
                return await ResponseWrapper.FailAsync("Username is already taken");

            var applicationUser = new ApplicationUser
            {
                FirstName = userRegistrationRequest.FirstName,
                LastName = userRegistrationRequest.LastName,
                UserName = userRegistrationRequest.UserName,
                Email = userRegistrationRequest.Email,
                EmailConfirmed = userRegistrationRequest.AutoConfirmEmail,
                PhoneNumber = userRegistrationRequest.PhoneNumer,
                IsActive = userRegistrationRequest.ActivateUser
            };
             // create password hasher and and assign it to newly created user
            var password = new PasswordHasher<ApplicationUser>();
            applicationUser.PasswordHash = password.HashPassword(applicationUser, userRegistrationRequest.Password);

            // create a user 
            var identityResult = await _userManager.CreateAsync(applicationUser);

            if (identityResult.Succeeded)
            {
                // assign user to role -- every new created role is assigned to guest role
                 await _userManager.AddToRoleAsync(applicationUser, AppRoles.Guest);

                await _emailSender.SendEmailAsync(userRegistrationRequest.Email, "Register", "Welcome To Our HR System Mamagement");

                return await ResponseWrapper<string>.SuccessAsync("User Registered Successfully");
            }

            return await ResponseWrapper.FailAsync(string.Join(",", GetIdentityErrorsDescription(identityResult)));
        }


        public async Task<IResponseWrapper> GetUserByIdAsync(string id)
        {
           var user = await _userManager.FindByIdAsync(id);

            if(user is { })  // means not null -- anther way to check null
            {
                var userResisterationResponse = _mapper.Map<UserRegistrationResponse>(user);
                return await ResponseWrapper<UserRegistrationResponse>.SuccessAsync(userResisterationResponse);
            }
            return await ResponseWrapper.FailAsync("Falied to Get the user");
        }

        public async Task<IResponseWrapper> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users.Any())
            {
                var userResisterationResponses = _mapper.Map<List<UserRegistrationResponse>>(users);
                return await ResponseWrapper<List<UserRegistrationResponse>>.SuccessAsync(userResisterationResponses);
            }
            return await ResponseWrapper.FailAsync("Falied to Get users");
        }

        public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest)
        {
            var userToUpdate = await _userManager.FindByIdAsync(updateUserRequest.UserId);
            if (userToUpdate is null)
                return await ResponseWrapper.FailAsync("Falied to Get users");

            userToUpdate.FirstName = updateUserRequest.FirstName;
            userToUpdate.LastName = updateUserRequest.LastName;
            userToUpdate.PhoneNumber = updateUserRequest.PhoneNumber;

            var identityResult = await _userManager.UpdateAsync(userToUpdate);

            if (identityResult.Succeeded)
            {
                return await ResponseWrapper<string>.SuccessAsync("User Updated Successfully");
            }

            return await ResponseWrapper.FailAsync(string.Join("," , GetIdentityErrorsDescription(identityResult)));
        }

        public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(changePasswordRequest.UserId);
            if (user is null)
                return await ResponseWrapper.FailAsync("Falied to Get users");

            // Attempt to change the password
            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);

            // If the password change fails, return a failure response with error details
            if (!result.Succeeded)
            {
                return await ResponseWrapper.FailAsync(string.Join("," , GetIdentityErrorsDescription(result)));
            }

            // Return a success response if the password was changed successfully
            return await ResponseWrapper.SuccessAsync("Password changed successfully.");


        }

        public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatusRequest)
        {
            var user = await _userManager.FindByIdAsync(changeUserStatusRequest.UserId);
            if (user is null)
                return await ResponseWrapper.FailAsync("Falied to Get users");

            user.IsActive = changeUserStatusRequest.Activate;

            var response = await _userManager.UpdateAsync(user);

            if (response.Succeeded)
            {
                return await ResponseWrapper<string>.SuccessAsync(changeUserStatusRequest.Activate ? "Activate User Successfully"
                    : "de-activate user successfuly");
            }

            return await ResponseWrapper<string>.FailAsync(changeUserStatusRequest.Activate ? "failed to Active User Successfully"
                    : "failed to de-active user successfuly");
        }

        private IReadOnlyList<string> GetIdentityErrorsDescription(IdentityResult identityResult)
        {
            return identityResult.Errors.Select(e=>e.Description).ToList();
        }

        public async Task<IResponseWrapper> GetRolesAsyn(string userId)
        {
            List<UserRolesModelView> userRolesViewModel = new();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return await ResponseWrapper.FailAsync("Falied to Get users");

            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(user , role.Name))
                {
                    userRolesViewModel.Add(new UserRolesModelView
                    {
                        RoleName = role.Name,
                        RoleDiscription = role.Description,
                        IsAssigned = true
                    });
                }
            }

            return await ResponseWrapper<List<UserRolesModelView>>.SuccessAsync(userRolesViewModel);
        }

        public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRoleRequest updateUserRoleRequest)
        {
            var user = await _userManager.FindByIdAsync(updateUserRoleRequest.UserId);
            if (user is null)
                return await ResponseWrapper.FailAsync("Falied to Get users");

            if (user.Email == AppCredentials.Email)
                return await ResponseWrapper.FailAsync("User Roles Update is not permitted");

            var currentUserRoles = await _userManager.GetRolesAsync(user);
            var newAssignedRoles = updateUserRoleRequest.Roles
                                                        .Where(r => r.IsAssigned)
                                                        .Select(r => r.RoleName)
                                                        .ToList();

            var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (currentLoggedInUser is null)
                return await ResponseWrapper.FailAsync("Falied to Get users");

            if (!await _userManager.IsInRoleAsync(currentLoggedInUser, AppRoles.Admin))
                return await ResponseWrapper.FailAsync("User Roles Update is not permitted");

            var rolesToRemove = currentUserRoles.Except(newAssignedRoles);
            var rolesToAdd = newAssignedRoles.Except(currentUserRoles);

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                    return await ResponseWrapper.FailAsync(string.Join(",", GetIdentityErrorsDescription(removeResult)));
            }

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                    return await ResponseWrapper.FailAsync(string.Join(",", GetIdentityErrorsDescription(addResult)));
            }

            return await ResponseWrapper.SuccessAsync("User Roles Updated Successfully.");
        }
    }
}
