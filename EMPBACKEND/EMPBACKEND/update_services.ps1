# PowerShell script to add AutoMapper to all services
$services = @(
    "AssessmentService.cs",
    "AssessmentAttemptService.cs", 
    "CertificateService.cs",
    "CourseProgressService.cs",
    "DailyGoalService.cs",
    "EnrollmentService.cs",
    "LearningPathService.cs",
    "LearningPlanService.cs",
    "NotificationService.cs",
    "OtpCodeService.cs",
    "UserAssessmentService.cs",
    "VideoService.cs",
    "VideoProgressService.cs",
    "VideoRequestService.cs"
)

foreach ($service in $services) {
    $path = "Services\$service"
    if (Test-Path $path) {
        $content = Get-Content $path -Raw
        
        # Add AutoMapper using statement if not present
        if ($content -notmatch "using AutoMapper;") {
            $content = $content -replace "(using EMPBACKEND\.Models;)", "`$1`nusing AutoMapper;"
        }
        
        # Add IMapper field and constructor parameter
        $content = $content -replace "(private readonly \w+ \w+;)", "`$1`n        private readonly IMapper _mapper;"
        $content = $content -replace "(\w+Service\([^)]+)\)", "`$1, IMapper mapper)"
        $content = $content -replace "(\{\s+)(_\w+ = \w+;)", "`$1`$2`n            _mapper = mapper;"
        
        Set-Content $path $content
        Write-Host "Updated $service"
    }
}