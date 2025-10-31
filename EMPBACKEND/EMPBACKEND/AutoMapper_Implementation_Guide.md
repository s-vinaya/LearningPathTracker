# AutoMapper Implementation Guide

## Overview
AutoMapper has been successfully implemented across the entire project to eliminate manual object mapping and reduce boilerplate code.

## Configuration

### 1. NuGet Packages Installed
- `AutoMapper` (13.0.1)
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.1)

### 2. Dependency Injection Setup
In `Program.cs`:
```csharp
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(AdditionalMappingProfile));
```

## Mapping Profiles

### 1. MappingProfile.cs
Contains core entity mappings:
- User ↔ UserDto, CreateUserDto, UpdateUserDto
- Course ↔ CourseDto, CreateCourseDto, UpdateCourseDto
- Enrollment ↔ EnrollmentDto, CreateEnrollmentDto
- Assessment ↔ AssessmentDto, CreateAssessmentDto, UpdateAssessmentDto
- Video ↔ VideoDto, CreateVideoDto, UpdateVideoDto
- LearningPath ↔ LearningPathDto, CreateLearningPathDto, UpdateLearningPathDto

### 2. AdditionalMappingProfile.cs
Contains additional entity mappings:
- Department ↔ DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto
- Category ↔ CategoryDto, CreateCategoryDto, UpdateCategoryDto
- AssessmentAttempt ↔ AssessmentAttemptDto, CreateAssessmentAttemptDto
- UserAssessment ↔ UserAssessmentDto, CreateUserAssessmentDto
- CourseProgress ↔ CourseProgressDto, CreateCourseProgressDto, UpdateCourseProgressDto
- DailyGoal ↔ DailyGoalDto, CreateDailyGoalDto, UpdateDailyGoalDto
- LearningPathCourse ↔ LearningPathCourseDto, CreateLearningPathCourseDto
- LearningPlan ↔ LearningPlanDto, CreateLearningPlanDto, UpdateLearningPlanDto
- Certificate ↔ CertificateDto, CreateCertificateDto
- Notification ↔ NotificationDto, CreateNotificationDto
- VideoRequest ↔ VideoRequestDto, CreateVideoRequestDto, UpdateVideoRequestDto
- VideoProgress ↔ VideoProgressDto, CreateVideoProgressDto, UpdateVideoProgressDto
- OtpCode ↔ OtpCodeDto, CreateOtpCodeDto

## Usage Examples

### In Controllers
```csharp
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users.Include(u => u.Department).ToListAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return Ok(userDtos);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var userDto = _mapper.Map<UserDto>(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _mapper.Map(dto, user); // Maps dto properties to existing user object
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}
```

### In Services
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        var user = _mapper.Map<User>(createDto);
        var createdUser = await _userRepository.CreateAsync(user);
        var result = await _userRepository.GetByIdAsync(createdUser.Id);
        return _mapper.Map<UserDto>(result!);
    }
}
```

## Key Mapping Features

### 1. Automatic Property Mapping
AutoMapper automatically maps properties with the same name and compatible types.

### 2. Custom Property Mapping
```csharp
CreateMap<User, UserDto>()
    .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));
```

### 3. Ignoring Properties
```csharp
CreateMap<CreateUserDto, User>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
```

### 4. Conditional Mapping
```csharp
CreateMap<UpdateUserDto, User>()
    .ForMember(dest => dest.Password, opt => opt.Ignore()) // Don't map password in updates
    .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
```

## Extension Methods
Custom extension methods in `Extensions/AutoMapperExtensions.cs`:

```csharp
// Map collection to list
var userDtos = _mapper.MapToList<UserDto>(users);

// Map with null check
var userDto = _mapper.MapOrDefault<UserDto>(user);

// Map to existing object
_mapper.MapTo(updateDto, existingUser);
```

## Benefits

1. **Reduced Boilerplate Code**: Eliminates manual property mapping
2. **Type Safety**: Compile-time checking of mappings
3. **Maintainability**: Centralized mapping configuration
4. **Performance**: Optimized mapping with caching
5. **Consistency**: Standardized mapping across the application

## Best Practices

1. **Profile Organization**: Separate profiles by domain/feature
2. **Explicit Mapping**: Use `ForMember` for complex mappings
3. **Validation**: Use `AssertConfigurationIsValid()` in tests
4. **Performance**: Use `ProjectTo<T>()` for LINQ queries when possible
5. **Null Handling**: Always handle null scenarios in custom mappings

## Testing AutoMapper Configuration

```csharp
[Test]
public void AutoMapper_Configuration_IsValid()
{
    var configuration = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<MappingProfile>();
        cfg.AddProfile<AdditionalMappingProfile>();
    });
    
    configuration.AssertConfigurationIsValid();
}
```

## Migration from Manual Mapping

All controllers and services have been updated to use AutoMapper instead of manual object mapping. The old manual mapping methods have been removed to maintain clean code.

## Next Steps

1. Add unit tests for mapping profiles
2. Consider using `ProjectTo<T>()` for database queries
3. Add custom value resolvers for complex mapping scenarios
4. Implement mapping for any new DTOs/Models added to the project