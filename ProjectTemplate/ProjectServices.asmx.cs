using System;
using System.Web;
using System.Web.Services;
using System.Data;
using Newtonsoft.Json;
using MySqlConnector;
//needed to get List objects
using System.Collections.Generic;

namespace ProjectTemplate
{
    [WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		////////////////////////////////////////////////////////////////////////
		///replace the values of these variables with your database credentials
		////////////////////////////////////////////////////////////////////////
		private string dbID = "440sum20221";
		private string dbPass = "440sum20221";
		private string dbName = "440sum20221";
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		///call this method anywhere that you need the connection string!
		////////////////////////////////////////////////////////////////////////
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}
		////////////////////////////////////////////////////////////////////////

		[WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
		public string LogOn(string uid, string pass)
		{
			//we return this flag to tell them if they logged in or not
			bool success = false;
			string isManager = "false";
			string fname = "";
			string lname = "";

			//our connection string comes from our web.config file like we talked about earlier
			//string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			string sqlConnectString = getConString();
			//here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
			//NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
			string sqlSelect = "SELECT id, ismanager, fname, lname FROM users WHERE email=@idValue and password=@passValue";

			//set up our connection object to be ready to use our connection string
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			//set up our command object to use our connection, and our query
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//tell our command to replace the @parameters with real values
			//we decode them because they came to us via the web so they were encoded
			//for transmission (funky characters escaped, mostly)
			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

			//a data adapter acts like a bridge between our command object and 
			//the data we are trying to get back and put in a table object
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//here's the table we want to fill with the results from our query
			DataTable sqlDt = new DataTable();
			//here we go filling it!
			sqlDa.Fill(sqlDt);
			//check to see if any rows were returned.  If they were, it means it's 
			//a legit account
			if (sqlDt.Rows.Count > 0)
			{
				//if we found an account, store the id and admin status in the session
				//so we can check those values later on other method calls to see if they 
				//are 1) logged in at all, and 2) and admin or not
				Session["id"] = sqlDt.Rows[0]["id"];
				Session["ismanager"] = sqlDt.Rows[0]["ismanager"];
				isManager = sqlDt.Rows[0]["ismanager"].ToString();
				success = true;
				fname = sqlDt.Rows[0]["fname"].ToString();
				lname = sqlDt.Rows[0]["lname"].ToString();
			}
			var result = new logOnResult();
			result.successful = success;
			result.isManager = isManager;
			result.fname = fname;
			result.lname = lname;


			//return the result!
			return JsonConvert.SerializeObject(result);
		}

		[WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
		public string LogOnAnonymous()
		{
			//we return this flag to tell them if they logged in or not
			bool success = false;
			string isManager = "false";

			//our connection string comes from our web.config file like we talked about earlier
			//string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			string sqlConnectString = getConString();
			//here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
			//NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
			string sqlSelect = "SELECT id, ismanager FROM users WHERE id=6";

			//set up our connection object to be ready to use our connection string
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			//set up our command object to use our connection, and our query
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//a data adapter acts like a bridge between our command object and 
			//the data we are trying to get back and put in a table object
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//here's the table we want to fill with the results from our query
			DataTable sqlDt = new DataTable();
			//here we go filling it!
			sqlDa.Fill(sqlDt);
			//check to see if any rows were returned.  If they were, it means it's 
			//a legit account
			if (sqlDt.Rows.Count > 0)
			{
				//if we found an account, store the id and admin status in the session
				//so we can check those values later on other method calls to see if they 
				//are 1) logged in at all, and 2) and admin or not
				Session["id"] = sqlDt.Rows[0]["id"];
				Session["ismanager"] = sqlDt.Rows[0]["ismanager"];
				isManager = sqlDt.Rows[0]["ismanager"].ToString();
				success = true;
			}
			var result = new logOnResult();
			result.successful = success;
			result.isManager = isManager;
			//return the result!
			return JsonConvert.SerializeObject(result);
		}

		[WebMethod(EnableSession = true)]
		public void CreateAccount(string uid, string pass, string firstName, string lastName, string isManager)
		{
			string sqlConnectString = getConString();
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "insert into users (email, password, fname, lname, ismanager) " +
				"values(@idValue, @passValue, @fnameValue, @lnameValue, @isManagerValue); SELECT LAST_INSERT_ID();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
			sqlCommand.Parameters.AddWithValue("@isManagerValue", HttpUtility.UrlDecode(isManager));

			//this time, we're not using a data adapter to fill a data table.  We're just
			//opening the connection, telling our command to "executescalar" which says basically
			//execute the query and just hand me back the number the query returns (the ID, remember?).
			//don't forget to close the connection!
			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

        [WebMethod(EnableSession = true)]
        public void CreatePost(string post, string department)
        {
            string sqlconnectstring = getConString();
            //the only thing fancy about this query is select last_insert_id() at the end.  all that
            //does is tell mysql server to return the primary key of the last inserted row.
            string sqlselect = "insert into posts(UserID,Post,Department,Datetime)" +
                               "values(@id,@post,@department,@datetime); select last_insert_id();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@id", HttpUtility.UrlDecode(Session["id"].ToString()));
			sqlCommand.Parameters.AddWithValue("@post", HttpUtility.UrlDecode(post));
			sqlCommand.Parameters.AddWithValue("@department", HttpUtility.UrlDecode(department));
			sqlCommand.Parameters.AddWithValue("@datetime", DateTime.Now);

			//this time, we're not using a data adapter to fill a data table.  We're just
			//opening the connection, telling our command to "executescalar" which says basically
			//execute the query and just hand me back the number the query returns (the ID, remember?).
			//don't forget to close the connection!
			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public void CreatePostAnonymously(string post, string department)
		{
			string sqlconnectstring = getConString();
			//the only thing fancy about this query is select last_insert_id() at the end.  all that
			//does is tell mysql server to return the primary key of the last inserted row.
			string sqlselect = "insert into posts(UserID,Post,Department,Datetime)" +
							   "values(@id,@post,@department,@datetime); select last_insert_id();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@id", HttpUtility.UrlDecode((Session["id"] = 6).ToString()));
			sqlCommand.Parameters.AddWithValue("@post", HttpUtility.UrlDecode(post));
			sqlCommand.Parameters.AddWithValue("@department", HttpUtility.UrlDecode(department));
			sqlCommand.Parameters.AddWithValue("@datetime", DateTime.Now);

			//this time, we're not using a data adapter to fill a data table.  We're just
			//opening the connection, telling our command to "executescalar" which says basically
			//execute the query and just hand me back the number the query returns (the ID, remember?).
			//don't forget to close the connection!
			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public bool LogOff()
		{
			//if they log off, then we remove the session.  That way, if they access
			//again later they have to log back on in order for their ID to be back
			//in the session!
			Session.Abandon();
			return true;
		}

        [WebMethod(EnableSession =true)]
		public Post[] GetPosts()
        {
            if (Session["id"] != null)
            {
				DataTable sqlDt = new DataTable("posts");

				//string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				string sqlConnectString = getConString();
				string sqlSelect = "Select PostID, UserID, Post, Department, DateTimes, Likes, Dislikes, Comments, Solved, Rejected from posts";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				//gonna use this to fill a data table
				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				//filling the data table
				sqlDa.Fill(sqlDt);

				//loop through each row in the dataset, creating instances
				//of our container class Post.  Fill each post with
				//data from the rows, then dump them in a list.
				List<Post> posts = new List<Post>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
                {
					posts.Add(new Post
					{ 
						id = Convert.ToInt32(sqlDt.Rows[i]["PostId"]),
						uid = Convert.ToInt32(sqlDt.Rows[i]["UserID"]),
						postText = sqlDt.Rows[i]["Post"].ToString(),
						department = sqlDt.Rows[i]["Department"].ToString(),
						postDate = Convert.ToDateTime(sqlDt.Rows[i]["DateTimes"]),
						likes = Convert.ToInt32(sqlDt.Rows[i]["Likes"]),
						dislikes = Convert.ToInt32(sqlDt.Rows[i]["Dislikes"]),
						hasComments = Convert.ToBoolean(sqlDt.Rows[i]["Comments"]),
						isSolved = Convert.ToBoolean(sqlDt.Rows[i]["Solved"]),
						isRejected = Convert.ToBoolean(sqlDt.Rows[i]["Rejected"])
					});
				}
				//convert the list of posts to an array and return!
				return posts.ToArray();
            }
            else
            {
				return new Post[0];
            }
		}
	}
}
