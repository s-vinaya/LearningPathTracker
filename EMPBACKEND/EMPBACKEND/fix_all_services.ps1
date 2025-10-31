# Fix all services to use AutoMapper properly
$services = Get-ChildItem "Services\*.cs" | Where-Object { $_.Name -notmatch "(UserService|CourseService|DepartmentService|CategoryService|AssessmentService)" }

foreach ($service in $services) {
    $content = Get-Content $service.FullName -Raw
    
    # Replace common manual mapping patterns with AutoMapper
    $content = $content -replace 'return \w+\.Select\([^}]+\}\);', 'return _mapper.Map<IEnumerable<$1>>($2);'
    $content = $content -replace 'new \w+Dto\s*\{[^}]+\}', '_mapper.Map<$1>($2)'
    $content = $content -replace '(\w+) != null \? new (\w+Dto)\s*\{[^}]+\} : null', '$1 != null ? _mapper.Map<$2>($1) : null'
    
    Set-Content $service.FullName $content
    Write-Host "Fixed $($service.Name)"
}