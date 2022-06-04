param
(
    #any set of values you want here
    [ComponentModel.DisplayName("Hallo zusammen")]
    [Parameter(Mandatory=$false, Position=0, HelpMessage='Aus dem Vornamen werden die restlichen Attribute erstellt für den Account')]
    [ValidateSet('Input','Output','Both','a','b','c','d',1,2)] 
    [string[]]$Input09BConstrainedSet='a'
)

begin {
    #do pre script checks, etc

} #/begin

process {


function start-countdown {
  param (
      $sleepintervalsec
   )

   foreach ($step in (1..$sleepintervalsec)) {
      write-progress -Activity "Waiting" -Status "Waiting - Press any key to stop" -SecondsRemaining ($sleepintervalsec-$step) -PercentComplete  ($step/$sleepintervalsec*100)
      Write-Verbose "Starting Process"
	Write-Host (Get-Date).tostring()
	Write-Host "Running as: $($env:username)"
	Write-Host "PSBoundParameters"
      start-sleep -seconds 1
   }
}

start-countdown -sleepintervalsec 1

return @{
    Test="Test"
}


} #/process

end {
    #useful for cleanup, or to write-output whatever you want to return to the user
} #/end

