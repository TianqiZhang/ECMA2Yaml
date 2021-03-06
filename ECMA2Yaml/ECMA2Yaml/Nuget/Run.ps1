﻿param (
    [parameter(mandatory=$true)]
    [hashtable]$ParameterDictionary
)

$currentDir = $($MyInvocation.MyCommand.Definition) | Split-Path
$ecma2yamlExeName = "ECMA2Yaml.exe"

# Main
$errorActionPreference = 'Stop'

$repositoryRoot = $ParameterDictionary.environment.repositoryRoot
$logFilePath = $ParameterDictionary.environment.logFile
$logOutputFolder = $ParameterDictionary.environment.logOutputFolder
$cacheFolder = $ParameterDictionary.environment.cacheFolder
$outputFolder = $ParameterDictionary.environment.outputFolder

$dependentFileListFilePath = $ParameterDictionary.context.dependentFileListFilePath
$changeListTsvFilePath = $ParameterDictionary.context.changeListTsvFilePath
$userSpecifiedChangeListTsvFilePath = $ParameterDictionary.context.userSpecifiedChangeListTsvFilePath

pushd $repositoryRoot
$repoBranch = $ParameterDictionary.environment.repositoryBranch

$repoUrl = & git config --get remote.origin.url
if ($repoUrl.EndsWith(".git"))
{
    $repoUrl = $repoUrl.Substring(0, $repoUrl.Length - 4)
}
if ([string]::IsNullOrEmpty($repoBranch))
{
    & git branch | foreach {
        if ($_ -match "^\* (.*)") {
            $repoBranch = $matches[1]
        }
    }
}
popd

$publicGitUrl = $repoUrl
$publicBranch = $repoBranch
if (-not [string]::IsNullOrEmpty($ParameterDictionary.environment.publishConfigContent.git_repository_url_open_to_public_contributors))
{
    $publicGitUrl = $ParameterDictionary.environment.publishConfigContent.git_repository_url_open_to_public_contributors
}
if (-not [string]::IsNullOrEmpty($ParameterDictionary.environment.publishConfigContent.git_repository_branch_open_to_public_contributors))
{
    $publicBranch = $ParameterDictionary.environment.publishConfigContent.git_repository_branch_open_to_public_contributors
}
echo "Using $repoUrl and $repoBranch as git url base"
echo "Using $publicGitUrl and $publicBranch as public git url base"

$jobs = $ParameterDictionary.docset.docsetInfo.ECMA2Yaml
if (!$jobs)
{
    $jobs = $ParameterDictionary.environment.publishConfigContent.ECMA2Yaml
}
if ($jobs -isnot [system.array])
{
    $jobs = @($jobs)
}
foreach($ecmaConfig in $jobs)
{
    $ecmaSourceXmlFolder = Join-Path $repositoryRoot $ecmaConfig.SourceXmlFolder
    $ecmaOutputYamlFolder = Join-Path $repositoryRoot $ecmaConfig.OutputYamlFolder
    $allArgs = @("-s", "$ecmaSourceXmlFolder",
	"-o", "$ecmaOutputYamlFolder",
	"-l", "$logFilePath",
	"-p",
	"--repoRoot", "$repositoryRoot",
	"--repoBranch", "$repoBranch",
	"--repoUrl", "$repoUrl",
	"--publicRepoBranch", "$publicBranch",
	"--publicRepoUrl", "$publicGitUrl");
    
    $processedGitUrl = $repoUrl -replace "https://","" -replace "/","_"
    $reportId = $ecmaConfig.id
    if (-not $reportId)
    {
        $reportId = $ParameterDictionary.docset.docsetInfo.docset_name
    }
    $undocumentedApiReport = Join-Path $outputFolder "UndocAPIReport_${processedGitUrl}_${branch}_${reportId}.xlsx"
    $allArgs += "--undocumentedApiReport"
    $allArgs += "$undocumentedApiReport"
	$yamlXMLMappingFile = Join-Path $logOutputFolder "build_file_path_to_repo_file_path.mapped.json"
	$allArgs += "--yamlXMLMappingFile"
	$allArgs += "$yamlXMLMappingFile"

    $fallbackRepoRoot = Join-Path $repositoryRoot _repo.en-us
    $ecmaFallbackSourceXmlFolder = Join-Path $fallbackRepoRoot $ecmaConfig.SourceXmlFolder
    $fallbackRepo = $null
    foreach($repo in $ParameterDictionary.environment.publishConfigContent.dependent_repositories)
    {
        if ($repo.path_to_root -eq "_repo.en-us")
        {
            $fallbackRepo = $repo
        }
    }
    if (-not (Test-Path $ecmaSourceXmlFolder) -and -not $fallbackRepo)
    {
        continue;
    }
    if ($fallbackRepo -and (Test-Path $ecmaFallbackSourceXmlFolder))
    {
        if ([string]::IsNullOrEmpty($ParameterDictionary.environment.skipPublishFilePath)) {
            $ParameterDictionary.environment.skipPublishFilePath = Join-Path $logOutputFolder "skip-publish-file-path.json"
        }
        $skipPublishFilePath = $ParameterDictionary.environment.skipPublishFilePath;
        $allArgs +=  "-skipPublishFilePath"
        $allArgs +=  "$skipPublishFilePath"

		# workaround for https://dev.azure.com/ceapex/Engineering/_workitems/edit/131942
		$enIncludePath = Join-Path $fallbackRepoRoot "includes"
		$locIncludePath = Join-Path $repositoryRoot "includes"
		& robocopy /xc /xn /xo $enIncludePath $locIncludePath
    }
    
    if ($ecmaConfig.Flatten)
    {
        $allArgs += "-f";
    }
    if ($ecmaConfig.StrictMode)
    {
        $allArgs += "-strict";
    }
    if ($ecmaConfig.SDPMode)
    {
        $allArgs += "-SDP";
    }
	if ($ecmaConfig.UWP)
    {
        $allArgs += "-UWP";
    }
	if ($ecmaConfig.NoVersioning)
    {
        $allArgs += "-NoVersioning";
    }
    if (-not [string]::IsNullOrEmpty($ecmaConfig.SourceMetadataFolder))
    {
        $ecmaSourceMetadataFolder = Join-Path $repositoryRoot $ecmaConfig.SourceMetadataFolder
        if (Test-Path $ecmaSourceMetadataFolder)
        {
            $allArgs += "-m";
            $allArgs += "$ecmaSourceMetadataFolder";
        }
    }

    $changeListFile = $ParameterDictionary.context.changeListTsvFilePath;
    if (-not [string]::IsNullOrEmpty($changeListFile) -and (Test-Path $changeListFile))
    {
        $newChangeList = $changeListFile -replace "\.tsv$",".mapped.tsv";
        $allArgs += "-changeList";
        $allArgs += "$changeListFile";
    }
    $userChangeListFile = $ParameterDictionary.context.userSpecifiedChangeListTsvFilePath;
    if (-not [string]::IsNullOrEmpty($userChangeListFile) -and (Test-Path $userChangeListFile))
    {
        $newUserChangeList = $userChangeListFile -replace "\.tsv$",".mapped.tsv";
        $allArgs += "-changeList";
        $allArgs += "$userChangeListFile";
    }

    $printAllArgs = [System.String]::Join(' ', $allArgs)
    $ecma2yamlExeFilePath = Join-Path $currentDir $ecma2yamlExeName
    echo "Executing $ecma2yamlExeFilePath $printAllArgs" | timestamp
    & "$ecma2yamlExeFilePath" $allArgs
    if ($LASTEXITCODE -ne 0)
    {
        exit $LASTEXITCODE
    }
    if (Test-Path $newChangeList)
    {
        $ParameterDictionary.context.changeListTsvFilePath = $newChangeList
    }
    if (Test-Path $newUserChangeList)
    {
        $ParameterDictionary.context.userSpecifiedChangeListTsvFilePath = $newUserChangeList
    }
    if (-not [string]::IsNullOrEmpty($ecmaConfig.id))
    {
        $tocPath = Join-Path $ecmaOutputYamlFolder "toc.yml"
        if (Test-Path $tocPath)
        {
            $newTocPath = Join-Path $ecmaOutputYamlFolder $ecmaConfig.id
            if (-not (Test-Path $newTocPath))
            {
                New-Item -ItemType Directory -Force -Path $newTocPath
            }
            Move-Item $tocPath $newTocPath -Force
        }
    }
}
