using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by ID: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                return null;
            }
            return MapToDto(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            _logger.LogInformation("Getting user by username: {Username}", username);
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User not found with username: {Username}", username);
                return null;
            }
            return MapToDto(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Getting user by email: {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", email);
                return null;
            }
            return MapToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users");
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user with username: {Username}", createUserDto.Username);

            var existingUserByUsername = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUserByUsername != null)
            {
                _logger.LogWarning("Username already exists: {Username}", createUserDto.Username);
                throw new InvalidOperationException($"Username '{createUserDto.Username}' is already taken");
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUserByEmail != null)
            {
                _logger.LogWarning("Email already exists: {Email}", createUserDto.Email);
                throw new InvalidOperationException($"Email '{createUserDto.Email}' is already registered");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                DateOfBirth = createUserDto.DateOfBirth,
                PhoneNumber = createUserDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = await _userRepository.CreateAsync(user);
            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
            return MapToDto(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("User not found for update with ID: {UserId}", id);
                return null;
            }

            existingUser.FirstName = updateUserDto.FirstName;
            existingUser.LastName = updateUserDto.LastName;
            existingUser.DateOfBirth = updateUserDto.DateOfBirth;
            existingUser.PhoneNumber = updateUserDto.PhoneNumber;
            existingUser.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            _logger.LogInformation("User updated successfully with ID: {UserId}", id);
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var result = await _userRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
            }
            else
            {
                _logger.LogWarning("User not found for deletion with ID: {UserId}", id);
            }
            return result;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };
        }
    }
}