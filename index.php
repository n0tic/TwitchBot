<?php
if(isset($_GET['ch'], $_GET['u']) && $_GET['ch'] != "" && ctype_alnum($_GET['ch'])) {
	header('Content-Type: application/json;charset=utf-8');
	$data = json_decode(file_get_contents('php://input'), true);
        
	$fp = fopen($_GET['ch'].'.json', 'w') or die("NoAccess");
	if(fwrite($fp, json_encode($data))) {
		echo "OK";
	} else {
		echo "NoWrite";
	}
	fclose($fp);
}
else if(isset($_GET['ch'], $_GET['d']) && $_GET['ch'] != "" && ctype_alnum($_GET['ch']))
{
    header('Content-Type: application/json;charset=utf-8');
    $data = NULL;
	
	if(file_exists($_GET['ch'].'.json'))
	{
		if(file_get_contents($_GET['ch'].'.json'))
		{
			$data = file_get_contents($_GET['ch'].'.json');
		}
		else
		{
			$data = "NoContent";
		}
		echo $data;
	}
    else
	{
		echo "NoFile";
	}
}
?>