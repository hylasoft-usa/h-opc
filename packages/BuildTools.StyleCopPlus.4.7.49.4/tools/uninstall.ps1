param($installPath, $toolsPath, $package, $project)

function Uninstall-StyleCopPlus {

    $itemGroupName = "StyleCopAdditionalAddinPaths"

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

    $project.Save()
}
 
Uninstall-StyleCopPlus