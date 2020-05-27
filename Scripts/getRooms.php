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
    //get Rooms
	$data = "";
	$sql = "SELECT * FROM TblRoom;";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//write the data into a string
        $data .= $row['RoomID'] . "," . $row['RoomName'] . "," . $row['L1ID'] . "," . $row['L2ID'] . "~";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}
	//get Languages
	$data = rtrim($data, '~');
	$data .= ";";
	$sql = "SELECT * FROM TblLanguage;";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//write the data into the string
        $data .= $row['LanguageID'] . "," . $row['LanguageName'] . "," . $row['LanguageNameEN'] . "~";
		}
    } else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}

	//echo that string
	$data = rtrim($data, '~');
	echo $data;
	mysqli_close($link);
?>