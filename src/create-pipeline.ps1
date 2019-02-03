#https://aws.amazon.com/blogs/devops/aws-building-a-secure-cross-account-continuous-delivery-pipeline/
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true, HelpMessage="Enter your config name defined in parameters.json: ")][string]$configName
)

$environment = Get-Content 'parameters.json' | Out-String | ConvertFrom-Json

# Parse the parameters file
$StageAccountId = $environment.$configName.AWS.StageAccountId
$StageAccountProfile = $environment.$configName.AWS.StageAccountProfile
$ProdAccountId = $environment.$configName.AWS.ProdAccountId
$ProdAccountProfile = $environment.$configName.AWS.ProdAccountProfile
$AppName = $environment.$configName.Application.Name
$ApprovalEmail = $environment.$configName.Application.ApprovalEmail
$GitHubUser = $environment.$configName.Application.GitHubUser
$GitHubRepoName = $environment.$configName.Application.GitHubRepoName
$GitHubRepoBranch = $environment.$configName.Application.GitHubRepoBranch
$GitHubToken = $environment.$configName.Application.GitHubToken
$EnableSlackChatOps = $environment.$configName.Application.EnableSlackChatOps
$SlackChannel = $environment.$configName.Application.SlackChannel
$SlackWebhook = $environment.$configName.Application.SlackWebhook
$SlackAppToken = $environment.$configName.Application.SlackAppToken
$BuildImage = $environment.$configName.CodeBuild.BuildImage
$ComputeType = $environment.$configName.CodeBuild.ComputeType

# Ensure all parameters are populated
While (!$StageAccountId) {
    $StageAccountId = Read-Host "StageAccountId is required"
}
While (!$StageAccountProfile) {
    $StageAccountProfile = Read-Host "StageAccountProfile is required"
}
While (!$AppName) {
    $AppName = Read-Host "AppName is required"
}
While (!$BuildImage) {
    $BuildImage = Read-Host "BuildImage is required"
}
If (!$BuildImage) {
    $BuildImage = "BUILD_GENERAL1_SMALL"
}
While (!$GitHubUser) {
    $GitHubUser = Read-Host "GitHubUser is required"
}
While (!$GitHubRepoName) {
    $GitHubRepoName = Read-Host "GitHubRepoName is required"
}
While (!$GitHubRepoBranch) {
    $GitHubRepoBranch = Read-Host "GitHubRepoBranch is required"
}
While (!$GitHubToken) {
    $GitHubToken = Read-Host "GitHubToken is required"
}

Clear-Host
Write-Host "This script will create the AWS CodePipeline with the following configuration:"
Write-Host ""
Write-Host "   AWS Account"
Write-Host "      StageProfile:       $StageAccountProfile [$StageAccountId]"
Write-Host ""
Write-Host "   CodeBuild"
Write-Host "      BuildImage:         $BuildImage"
Write-Host "      ComputeType:        $ComputeType"
$S3Bucket = "codepipeline-$AppName-$StageAccountId-us-west-2"
Write-Host "      S3Bucket:           $S3Bucket"
Write-Host ""
Write-Host "   Application"
Write-Host "      AppName:            $AppName"
Write-Host "      GitHubUser:         $GitHubUser"
Write-Host "      GitHubRepoName:     $GitHubRepoName"
Write-Host "      GitHubRepoBranch:   $GitHubRepoBranch"
$FilteredGitHubToken = $GitHubToken.Substring($GitHubToken.Length - 4)
Write-Host "      GitHubToken:        [****$FilteredGitHubToken]"
Write-Host ""
Write-Host "Press [enter] to continue (Ctrl+C to exit)" -ForegroundColor Yellow -NoNewline
Read-Host

aws cloudformation deploy `
    --stack-name $AppName-codepipeline `
    --template-file ./code-pipeline.yaml `
    --parameter-overrides 
        AppName=$AppName `
        BuildImage=$BuildImage `
        GitHubUser=$GitHubUser `
        GitHubRepoName=$GitHubRepoName `
        GitHubToken=$GitHubToken `
        GitHubRepoBranch=$GitHubRepoBranch `
    --capabilities CAPABILITY_NAMED_IAM `
    --profile=$StageAccountProfile
