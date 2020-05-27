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

    $intTypeID = $_REQUEST['interactionTypeID'];
    $itemID = $_REQUEST['itemID'];
    $convID = $_REQUEST['conversationID'];
    $roomID = $_REQUEST['roomID'];
    $userID = $_REQUEST['userID'];
    $correct = $_REQUEST['correct'];


    //take the data send and write it into a new record.
    $sql = "INSERT INTO tblinteraction (interactionTypeID, itemID, conversationID, roomID, userID, correct) VALUES ($intTypeID, $itemID, $convID, $roomID, $userID, $correct)";
	if(mysqli_query($link, $sql)) {
		echo "successful";
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
	mysqli_close($link);
?>