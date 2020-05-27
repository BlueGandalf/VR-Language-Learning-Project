<?php
    // Configuration
    $hostname = 'localhost';
    $username = 'cops';
    $password = 'Ha2YpTChsJ';
    $database = 'cops';
	
	//get the data that was sent.
    $roomID = $_REQUEST["RoomID"];
    $userID = $_REQUEST["UserID"];
    
    $link = mysqli_connect($hostname , $username , $password ,$database);
	if($link === false) {
		die("ERROR: Could not connect. " . mysqli_connect_error());
	}

	//get the most recent translation value in the interaction database for the specified user in the specified room.  
    $data = "";
    $sql = "SELECT SettingValue FROM tblinteraction WHERE SettingName = \"Translations\" AND tblinteraction.userID = $userID AND RoomID = $roomID ORDER BY tblinteraction.datetime DESC LIMIT 1";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
            $data .= $row['SettingValue'] . ",";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}
	//get the most recent audio value in the interaction database for the specified user in the specified room.
	$sql = "SELECT SettingValue FROM tblinteraction WHERE SettingName = \"Audio\" AND tblinteraction.userID = $userID AND RoomID = $roomID ORDER BY tblinteraction.datetime DESC LIMIT 1";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
            $data .= $row['SettingValue'] . ",";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}
	//get the most recent audioOnly setting value in the interaction database for the specified user in the specified room.
    $sql = "SELECT SettingValue FROM tblinteraction WHERE SettingName = \"AudioOnly\" AND tblinteraction.userID = $userID AND RoomID = $roomID ORDER BY tblinteraction.datetime DESC LIMIT 1";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
            $data .= $row['SettingValue'] . ",";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}
	//get the most recent guided learning setting value in the interaction database for the specified user in the specified room.
    $sql = "SELECT SettingValue FROM tblinteraction WHERE SettingName = \"GuidedLearning\" AND tblinteraction.userID = $userID AND RoomID = $roomID ORDER BY tblinteraction.datetime DESC LIMIT 1";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
            $data .= $row['SettingValue'] . ",";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}
	//get the most recent audio volume value in the interaction database for the specified user in the specified room.
    $sql = "SELECT SettingValue FROM tblinteraction WHERE SettingName = \"Volume\" AND tblinteraction.userID = $userID AND RoomID = $roomID ORDER BY tblinteraction.datetime DESC LIMIT 1";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
            $data .= $row['SettingValue'] . ",";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}

	//echo the string of data back. 
	$data = rtrim($data, ',');
	echo $data;
	mysqli_close($link);
?>