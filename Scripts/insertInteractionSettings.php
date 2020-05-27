<?php
    // Configuration
    $hostname = 'localhost';
    $username = 'cops';
    $password = 'Ha2YpTChsJ';
    $database = 'cops';
	
	$link = mysqli_connect($hostname , $username , $password ,$database);
	if($link === false) {
		die("ERROR: Could not connect. " . mysqli_connect_error());
	}
    //get the data that was sent
    $intTypeID = $_REQUEST['interactionTypeID'];
    $roomID = $_REQUEST['roomID'];
    $userID = $_REQUEST['userID'];
    $settingName1 = $_REQUEST['settingName1'];
    $settingValue1 = $_REQUEST['settingValue1'];
    $settingName2 = $_REQUEST['settingName2'];
    $settingValue2 = $_REQUEST['settingValue2'];
    $settingName3 = $_REQUEST['settingName3'];
    $settingValue3 = $_REQUEST['settingValue3'];
    $settingName4 = $_REQUEST['settingName4'];
    $settingValue4 = $_REQUEST['settingValue4'];
    $settingName5 = $_REQUEST['settingName5'];
    $settingValue5 = $_REQUEST['settingValue5'];
    
    //insert a new record for each setting, 1 through 5.
    $sql = "INSERT INTO tblinteraction (interactionTypeID, roomID, userID, settingName, settingValue) VALUES ($intTypeID, $roomID, $userID, \"$settingName1\", \"$settingValue1\"); ";
    if(mysqli_query($link, $sql)) {
		echo "successful";
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    $sql = "INSERT INTO tblinteraction (interactionTypeID, roomID, userID, settingName, settingValue) VALUES ($intTypeID, $roomID, $userID, \"$settingName2\", \"$settingValue2\"); ";
    if(mysqli_query($link, $sql)) {
		echo "successful";
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    $sql = "INSERT INTO tblinteraction (interactionTypeID, roomID, userID, settingName, settingValue) VALUES ($intTypeID, $roomID, $userID, \"$settingName3\", \"$settingValue3\"); ";
    if(mysqli_query($link, $sql)) {
		echo "successful";
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    $sql = "INSERT INTO tblinteraction (interactionTypeID, roomID, userID, settingName, settingValue) VALUES ($intTypeID, $roomID, $userID, \"$settingName4\", \"$settingValue4\"); ";
    if(mysqli_query($link, $sql)) {
		echo "successful";
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    $sql = "INSERT INTO tblinteraction (interactionTypeID, roomID, userID, settingName, settingValue) VALUES ($intTypeID, $roomID, $userID, \"$settingName5\", \"$settingValue5\");";
	if(mysqli_query($link, $sql)) {
		echo "successful";
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
	mysqli_close($link);
?>