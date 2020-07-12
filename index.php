<?php
if(isset($_GET['ch'], $_GET['bot'], $_GET['u']) && $_GET['ch'] != "" && ctype_alnum($_GET['ch']) && $_GET['bot'] != "" && ctype_alnum($_GET['bot'])) {
	header('Content-Type: application/json;charset=utf-8');
	$data = json_decode(file_get_contents('php://input'), true);
        
	$fp = fopen($_GET['bot'].'_'.$_GET['ch'].'.json', 'w') or die("NoAccess");
	if(fwrite($fp, json_encode($data))) {
		die("OK");
	} else {
		die("NoWrite");
	}
	fclose($fp);
}
else if(isset($_GET['ch'], $_GET['bot'], $_GET['d']) && $_GET['ch'] != "" && ctype_alnum($_GET['ch']) && $_GET['bot'] != "" && ctype_alnum($_GET['bot']))
{
    header('Content-Type: application/json;charset=utf-8');
    $data = NULL;
	
	if(file_exists($_GET['bot'].'_'.$_GET['ch'].'.json'))
	{
		if(file_get_contents($_GET['bot'].'_'.$_GET['ch'].'.json'))
		{
			$data = file_get_contents($_GET['bot'].'_'.$_GET['ch'].'.json');
		}
		else
		{
			$data = "NoContent";
		}
		die($data);
	}
    else
	{
		die("NoFile");
	}
}
die("This should not be visible.");
?>