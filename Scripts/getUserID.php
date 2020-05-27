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
	
    $UserName = $_REQUEST['UserName'];
    $UserID = -1;

    //check username exists
	$sql = "SELECT * FROM TblUser WHERE UserName = '$UserName'";
	if( $result = mysqli_query($link, $sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//if it exists, write it to a variable.
			$UserID = $row["UserID"];
		}
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
	}
	
	//if the user doesn't exist yet it will be -1 and so we insert a new user and user that userid.
    if( $UserID == -1 ){
        $sql = "INSERT INTO TblUser (UserName) VALUES ('$UserName')";
	if(mysqli_query($link, $sql)) {
		//done 
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    }


    //check username exists
	$sql = "SELECT * FROM TblUser WHERE UserName = '$UserName'";
	if( $result = mysqli_query($link, $sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			$UserID = $row["UserID"];
		}
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }

	//echo the id back.
    echo $UserID;
	mysqli_close($link);
?>