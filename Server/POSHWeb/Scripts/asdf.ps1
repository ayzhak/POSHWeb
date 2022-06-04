param(
    [Parameter(Mandatory=$true, Position=0, HelpMessage='Aus dem Vornamen werden die restlichen Attribute erstellt für den Account')]
    [string]$GivenName,
    [Parameter(Mandatory=$true, Position=1, HelpMessage='Aus dem Nachnamen werden die restlichen Attribute erstellt für den Account')]
    [string]$Surname,
    [Parameter(Mandatory=$true, Position=2, HelpMessage='Diese wird in der AD hinterlegt')]
    [string]$JobPosition,
    [Parameter(Mandatory=$true, Position=3, HelpMessage='Bitte eine Firma auswaehlen')]
    [ValidateSet('me','sm')]
    [string]$OrganisationAbbrevation,
    [Parameter(Position=1, HelpMessage='Erstellt ein Postfach auf dem Exchange Server')]
    [bool]$EMail,
    [string]$PhoneMobile,
    [ValidatePattern("[0-9]{3}")]
    [string]$InternalNumber,
    [ValidatePattern("[0-9]{3}")]
    [string]$Password

)

cd $PSScriptRoot
Import-Module .\NTFSSecurity
Import-Module .\ITTools

function Create-Username {
    param (
        [Parameter(Mandatory=$true, Position=0)]
        [string]
        $Firstname,
        [Parameter(Mandatory=$true, Position=1)]
        [string]
        $Lastname,
        $Suffix = $null
    )

    begin {
        $Firstname = Replace-Umlaut -Text $Firstname
        $Lastname = Replace-Umlaut -Text $Lastname
    }

    process {
        return "$($Firstname.ToLower().ToCharArray()[0]).$($Lastname.ToLower())$Suffix"
    }
}

$Config = Get-ConfigFromJson -Path ".\config.json"
$Organisation = Get-Organisation -Config $Config -Abbrevation $OrganisationAbbrevation
$Initals = Create-UserInitals -Firstname $GivenName -Lastname $Surname
$Password = Get-DefaultPassword -Config $Config
$Username = Create-Username -Firstname $GivenName -Lastname $Surname
$DisplayName = "$GivenName $Surname - $($Organisation.displayName)"
$Name = "$GivenName $Surname"

$Suffix = 0
while(Get-ADUser -Filter "sAMAccountName -like '$Username'") {
    $Suffix++
    $Username = Create-Username -Firstname $GivenName -Lastname $Surname -Suffix $Suffix
    $DisplayName = "$GivenName $Surname $Suffix - $($Organisation.displayName)"
    $Name = "$GivenName $Surname $Suffix"
}

# Determine which phone number to use
$OfficePhone = $Organisation.contact.phone
if($InternalNumber){$OfficePhone = $InternalNumber}
if($MobilePhone){$OfficePhone = $MobilePhone}

New-ADUser `
    -Enabled $true `
    -SamAccountName $Username `
    -UserPrincipalName $Username `
    -ChangePasswordAtLogon $true `
    -AccountPassword $Password `
    -GivenName $GivenName `
    -Surname $Surname `
    -Name $Name `
    -DisplayName $DisplayName `
    -Title $JobPosition `
    -Initials $Initals `
    -OfficePhone $OfficePhone `
    -MobilePhone $MobilePhone `
    -Company $Organisation.name `
    -HomePage $Organisation.contact.website `
    -Organization $Organisation.name `
    -Country $Organisation.contact.country `
    -State $Organisation.contact.canton `
    -PostalCode $Organisation.contact.postcode -City $Organisation.contact.city `
    -StreetAddress $Organisation.contact.address `
    -Description "Generated with Script"
    #-Path $Organisation.OrganisationUnits.Users `

if($InternalNumber) {
    Get-ADuser -Identity $Username | Set-ADuser -Replace @{ipPhone="$InternalNumber"}
}

# Add user to basic groups
Remove-Module ITTools