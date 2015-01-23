param($installPath, $toolsPath, $package, $project)

Function GetRelativeUri($rootPath, $relativePath, $appendToRelativePath)
{
    if($rootPath -eq $null)
    {
        return $null
    }

    if($relativePath -eq $null)
    {
        return $null
    }

    $rootUri = new-object system.Uri($rootPath)
    $targetPath = $relativePath

    # If appendToRelativePath is provided then use it
    if($appendToRelativePath -ne $null)
    {
        $targetPath = [io.path]::Combine($relativePath, $appendToRelativePath)
    }

    $targetUri = new-object system.Uri($targetPath)
    $relativeUri = $rootUri.MakeRelativeUri($targetUri)

    return $relativeUri
}

function Install-StyleCopPlus {

    $itemGroupName = "StyleCopAdditionalAddinPaths"

    $relativeToolUri = GetRelativeUri $project.FullName $installPath"\tools"
    $relativeToolUri = $relativeToolUri.ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

    $buildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) |
    Select-Object -First 1

    # Remove existing item group
    $existingItemGroups = $buildProject.Xml.ItemGroups |
        Where-Object { $_.Label -like "$itemGroupName" }

    if ($existingItemGroups) {
        $existingItemGroups |
            ForEach-Object {	
                $buildProject.Xml.RemoveChild($_) | Out-Null
        }
    }

    # Add new item group
    $newItemGroup = $buildProject.Xml.AddItemGroup()
    $newItemGroup.Label = $itemGroupName

    # Add new item element
    $itemElement = $buildProject.Xml.CreateItemElement("StyleCopAdditionalAddinPaths")
    $itemElement.Include = $relativeToolUri
    
    # Append to item group
    $newItemGroup.AppendChild($itemElement)

    # Hide from VisualStudio   
    $itemElement.AddMetadata("Visible", "false")

    $project.Save()
}
 
Install-StyleCopPlus