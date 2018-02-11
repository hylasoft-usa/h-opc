Write-Host 'Configure Test Project:'

$baseConfig = @'
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="UATestEndpoint" value="${uaEndpoint}"/>
  </appSettings>
</configuration>
'@

# Configure Test UA server endpoint
$uaAddress = Read-Host -Prompt 'Enter the address of your UA Test server (default: "opc.tcp://localhost:61210/UA/SampleServer")'

if ([string]::IsNullOrEmpty($uaAddress))
{
  $uaAddress = 'opc.tcp://localhost:61210/UA/SampleServer'
}

$baseConfig = $baseConfig.Replace('${uaEndpoint}', $uaAddress)

$baseConfig | Out-File 'tests\App.config'

Write-Host 'Configured project successfully'
