<?php

class DebugUtils
{
	public static function VarDump($expression)
	{
		$dump = print_r($expression, true);
				
		echo "<code>";
		$dump = str_replace("\n","<br/>", $dump);
		$dump = str_replace("  ","&nbsp;&nbsp;", $dump);
		echo $dump;
		echo "</code>";
	}	
}