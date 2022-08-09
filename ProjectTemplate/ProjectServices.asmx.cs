using System;
using System.Web;
using System.Web.Services;
using System.Data;
using Newtonsoft.Json;
using MySqlConnector;

using System.Collections.Generic;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		
		private string dbID = "440sum20221";
		private string dbPass = "440sum20221";
		private string dbName = "440sum20221";

		private string getConString()
		{
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName + "; UID=" + dbID + "; PASSWORD=" + dbPass;
		}
		

		[WebMethod(EnableSession = true)] 
		public string LogOn(string uid, string pass)
		{
			
			bool success = false;
			string isManager = "false";
			string fname = "";
			string lname = "";
			string id = "";

			string sqlConnectString = getConString();
			
			string sqlSelect = "SELECT id, ismanager, fname, lname FROM users WHERE email=@idValue and password=@passValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

			
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			DataTable sqlDt = new DataTable();
			
			sqlDa.Fill(sqlDt);
			
			if (sqlDt.Rows.Count > 0)
			{
			
				Session["id"] = sqlDt.Rows[0]["id"];
				Session["ismanager"] = sqlDt.Rows[0]["ismanager"];
				isManager = sqlDt.Rows[0]["ismanager"].ToString();
				success = true;
				fname = sqlDt.Rows[0]["fname"].ToString();
				lname = sqlDt.Rows[0]["lname"].ToString();
				id = sqlDt.Rows[0]["id"].ToString();
			}
			var result = new logOnResult();
			result.successful = success;
			result.isManager = isManager;
			result.fname = fname;
			result.lname = lname;
			result.id = id;


			//return the result!
			return JsonConvert.SerializeObject(result);
		}

		[WebMethod(EnableSession = true)]
		public void CreateAccount(string uid, string pass, string firstName, string lastName, string isManager)
		{
			string sqlConnectString = getConString();
			
			string sqlSelect = "insert into users (email, password, fname, lname, ismanager) " +
				"values(@idValue, @passValue, @fnameValue, @lnameValue, @isManagerValue); SELECT LAST_INSERT_ID();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
			sqlCommand.Parameters.AddWithValue("@isManagerValue", HttpUtility.UrlDecode(isManager));

			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
			
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
			
			string sqlselect = "insert into posts(UserID,Post,Department,DateTimes)" +
							   "values(@id,@post,@department,@datetime); select last_insert_id();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@id", HttpUtility.UrlDecode(Session["id"].ToString()));
			sqlCommand.Parameters.AddWithValue("@post", HttpUtility.UrlDecode(post));
			sqlCommand.Parameters.AddWithValue("@department", HttpUtility.UrlDecode(department));
			sqlCommand.Parameters.AddWithValue("@datetime", DateTime.Now);

			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
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
			
			string sqlselect = "insert into posts(UserID,Post,Department,DateTimes)" +
							   "values(@id,@post,@department,@datetime); select last_insert_id();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@id", HttpUtility.UrlDecode((Session["id"] = 6).ToString()));
			sqlCommand.Parameters.AddWithValue("@post", HttpUtility.UrlDecode(post));
			sqlCommand.Parameters.AddWithValue("@department", HttpUtility.UrlDecode(department));
			sqlCommand.Parameters.AddWithValue("@datetime", DateTime.Now);

			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public bool LogOff()
		{
			
			Session.Abandon();
			return true;
		}

		[WebMethod(EnableSession = true)]
		public void DeletePost(int postID)
		{
			DeleteCommentsByPostID(postID);
			string sqlconnectstring = getConString();
			
			string sqlselect = "DELETE FROM `440sum20221`.`posts` WHERE PostID =" + postID + ";";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public void DeleteCommentsByPostID(int postID)
		{
			string sqlconnectstring = getConString();
			
			string sqlselect = "DELETE FROM `440sum20221`.`comments` WHERE PostID = " + postID + ";";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public void DeleteCommentsByCommentID(int commentID)
		{
			string sqlconnectstring = getConString();
			
			string sqlselect = "DELETE FROM `440sum20221`.`comments` WHERE CommentID = " + commentID + ";";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			
			sqlConnection.Open();
			
			try
			{
				int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}



		[WebMethod(EnableSession = true)]
		public posts[] GetPost()
		{
			
            if (Session["id"] != null)
            {
				var id = Session["id"].ToString();
                DataTable sqlDt = new DataTable("posts");

				string sqlConnectString = getConString();
                string sqlSelect = "select posts.PostID, posts.UserID, posts.Post, posts.DateTimes, posts.Solved, posts.Department, users.fname, users.lname, users.email, " +
								   "posts.Archived, (select ifnull(sum(IsLike), 0) " +
                                   "from votes where PostID = posts.postid) as isliketotal, (select ifnull(sum(IsDislike),0) " +
                                   "from votes where PostID = posts.postid) as isdisliketotal, (select IF(islike = 1, 'Like', 'Dislike') from votes where postid = posts.postid and userid = " + id + ") as yourvote from posts inner join users on posts.UserID = users.id where posts.Solved = false and posts.Archived = false order by posts.DateTimes desc"; 

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                sqlDa.Fill(sqlDt);

              
                List<posts> posts = new List<posts>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					posts.Add(new posts
					{
						postId = Convert.ToInt32(sqlDt.Rows[i]["postId"]),
						userId = Convert.ToInt32(sqlDt.Rows[i]["userId"]),
						post = sqlDt.Rows[i]["Post"].ToString(),
						department = sqlDt.Rows[i]["Department"].ToString(),
						date = Convert.ToDateTime(sqlDt.Rows[i]["DateTimes"]).ToString("MM/dd/yyyy hh:mm tt"),
						firstName = sqlDt.Rows[i]["fname"].ToString(),
						lastName = sqlDt.Rows[i]["lname"].ToString(),
						email = sqlDt.Rows[i]["email"].ToString(),
						likes = Convert.ToInt32(sqlDt.Rows[i]["isliketotal"]),
						dislikes = Convert.ToInt32(sqlDt.Rows[i]["isdisliketotal"]),
						yourvote = sqlDt.Rows[i]["yourvote"].ToString(),
						isSolved = Convert.ToBoolean(sqlDt.Rows[i]["Solved"]),
						isArchived = Convert.ToBoolean(sqlDt.Rows[i]["Archived"]),
						activeUserID = Convert.ToInt32(Session["id"].ToString())
					});
                }
                return posts.ToArray();
            }
            else
            {
                return new posts[0];
            }
        }

		[WebMethod(EnableSession = true)]
		public posts[] GetSolved()
		{
			
			if (Session["id"] != null)
			{
				var id = Session["id"].ToString();
				DataTable sqlDt = new DataTable("posts");

				string sqlConnectString = getConString();
				string sqlSelect = "select posts.PostID, posts.UserID, posts.Post, posts.DateTimes, posts.Solved, posts.Department, posts.Archived, users.fname, users.lname, users.email, " +
								   "(select ifnull(sum(IsLike), 0) " +
								   "from votes where PostID = posts.postid) as isliketotal, (select ifnull(sum(IsDislike),0) " +
								   "from votes where PostID = posts.postid) as isdisliketotal, (select IF(islike = 1, 'Like', 'Dislike') from votes where postid = posts.postid and userid = " + id + ") as yourvote from posts inner join users on posts.UserID = users.id where posts.Solved = true and posts.Archived = false order by posts.DateTimes desc";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				sqlDa.Fill(sqlDt);

				
				List<posts> solvedPosts = new List<posts>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					solvedPosts.Add(new posts
					{
						postId = Convert.ToInt32(sqlDt.Rows[i]["postId"]),
						userId = Convert.ToInt32(sqlDt.Rows[i]["userId"]),
						post = sqlDt.Rows[i]["Post"].ToString(),
						department = sqlDt.Rows[i]["Department"].ToString(),
						date = Convert.ToDateTime(sqlDt.Rows[i]["DateTimes"]).ToString("MM/dd/yyyy hh:mm tt"),
						firstName = sqlDt.Rows[i]["fname"].ToString(),
						lastName = sqlDt.Rows[i]["lname"].ToString(),
						email = sqlDt.Rows[i]["email"].ToString(),
						likes = Convert.ToInt32(sqlDt.Rows[i]["isliketotal"]),
						dislikes = Convert.ToInt32(sqlDt.Rows[i]["isdisliketotal"]),
						yourvote = sqlDt.Rows[i]["yourvote"].ToString(),
						isSolved = Convert.ToBoolean(sqlDt.Rows[i]["Solved"]),
						isArchived = Convert.ToBoolean(sqlDt.Rows[i]["Archived"])
					});
				}
				return solvedPosts.ToArray();
			}
			else
			{
				return new posts[0];
			}
		}

		[WebMethod(EnableSession = true)]
		public Comments[] GetComments(int postID)
		{
			if (Session["id"] != null)
			{
				DataTable sqlDt = new DataTable("posts");

				string sqlConnectString = getConString();
				string sqlSelect = "SELECT c.`CommentID`,c.`UserID`,c.`PostID`,c.`Comment`,c.`DateTime`,u.fname,u.lname FROM `440sum20221`.`comments` c INNER JOIN users u on u.id = c.UserID  WHERE c.PostID = " + postID + " ORDER BY c.`DateTime` DESC;";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				sqlDa.Fill(sqlDt);

			
				List<Comments> comments = new List<Comments>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					comments.Add(new Comments
					{
						UserID = Convert.ToInt32(sqlDt.Rows[i]["UserID"]),
						FirstName = sqlDt.Rows[i]["fName"].ToString(),
						LastName = sqlDt.Rows[i]["lName"].ToString(),
						CommentID = Convert.ToInt32(sqlDt.Rows[i]["CommentID"]),
						Comment = sqlDt.Rows[i]["Comment"].ToString(),
						PostID = Convert.ToInt32(sqlDt.Rows[i]["PostID"]),
						DateTime = Convert.ToDateTime(sqlDt.Rows[i]["DateTime"]).ToString("MM/dd/yyyy hh:mm tt"),
						ActiveUserID = Convert.ToInt32(Session["id"].ToString())
					}); ;
				}
				return comments.ToArray();
			}
			else
			{
				return new Comments[0];
			}
		}

		[WebMethod(EnableSession = true)]
		public void CreateComment(string comment, int postID)
		{
			string sqlconnectstring = getConString();
			
			string sqlselect = "insert into comments(UserID,PostID,Comment,Datetime)" +
							   "values(@UserID,@PostID,@Comment,@DateTime); select last_insert_id();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@UserID", HttpUtility.UrlDecode(Session["id"].ToString()));
			sqlCommand.Parameters.AddWithValue("@PostID", postID);
			sqlCommand.Parameters.AddWithValue("@Comment", HttpUtility.UrlDecode(comment));
			sqlCommand.Parameters.AddWithValue("@DateTime", DateTime.Now);

			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public void CreateSolvedPost(int postID)
		{
			string sqlconnectstring = getConString();
			
			string sqlselect = "UPDATE `440sum20221`.`posts` SET `Solved` = true WHERE `PostID` = " + postID + ";";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);


			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
        public void CreateVote(string postid, string like, string dislike)
        {
			var id = Session["id"].ToString();
			string sqlconnectstring = getConString();
			
			string sqlselect = "create_vote";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);
			sqlCommand.CommandType = CommandType.StoredProcedure;

			sqlCommand.Parameters.Add("postidnum", MySqlDbType.Int32).Value = postid;
            sqlCommand.Parameters.Add("useridnum", MySqlDbType.Int32).Value = id;
            sqlCommand.Parameters.Add("upvote", MySqlDbType.Bool).Value = like;
            sqlCommand.Parameters.Add("downvote", MySqlDbType.Bool).Value = dislike;
			
			sqlConnection.Open();
		
			try
			{
				sqlCommand.ExecuteScalar();
                
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

		[WebMethod(EnableSession = true)]
		public posts[] FilterPostsDepartment(string dep)
		{		
			if (Session["id"] != null)
			{
				var id = Session["id"].ToString();
				DataTable sqlDt = new DataTable("posts");

				string sqlConnectString = getConString();
								
				string sqlSelect = "Select p.PostID, p.UserID, u.fname, u.lname, p.Post, p.Department, p.DateTimes, p.Solved, p.Archived, (select ifnull(sum(IsLike), 0) " +
								   "from votes where PostID = p.postid) as isliketotal, (select ifnull(sum(IsDislike),0) " +
								   "from votes where PostID = p.postid) as isdisliketotal, (select IF(islike = 1, 'Like', 'Dislike') from votes where postid = p.postid and userid = " + id + ") as yourvote from posts p inner join users u on u.id = p.UserID Where p.Department=@depValue and p.Solved = false and p.Archived = false order by DateTimes DESC";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@depValue", HttpUtility.UrlDecode(dep));

				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				sqlDa.Fill(sqlDt);

				List<posts> fposts = new List<posts>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					fposts.Add(new posts
					{
						postId = Convert.ToInt32(sqlDt.Rows[i]["PostId"]),
						userId = Convert.ToInt32(sqlDt.Rows[i]["UserID"]),
						firstName = sqlDt.Rows[i]["fname"].ToString(),
						lastName = sqlDt.Rows[i]["lname"].ToString(),
						post = sqlDt.Rows[i]["Post"].ToString(),
						department = sqlDt.Rows[i]["Department"].ToString(),
						date = Convert.ToDateTime(sqlDt.Rows[i]["DateTimes"]).ToString("MM/dd/yyyy hh:mm tt"),
						yourvote = sqlDt.Rows[i]["yourvote"].ToString(),
						likes = Convert.ToInt32(sqlDt.Rows[i]["isliketotal"]),
						dislikes = Convert.ToInt32(sqlDt.Rows[i]["isdisliketotal"]),
						isSolved = Convert.ToBoolean(sqlDt.Rows[i]["Solved"]),
						isArchived = Convert.ToBoolean(sqlDt.Rows[i]["Archived"])
					});
				}
				return fposts.ToArray();
			}
			else
			{
				return new posts[0];
			}
		}

		[WebMethod(EnableSession = true)]
		public posts[] FilterPostsPopular()
		{
			if (Session["id"] != null)
			{
				var id = Session["id"].ToString();
				DataTable sqlDt = new DataTable("posts");

				string sqlConnectString = getConString();
				string sqlSelect = "Select p.PostID, p.UserID, u.fname, u.lname, p.Post, p.Department, p.DateTimes, p.Solved, p.Archived, (select ifnull(sum(IsLike), 0) " +
				   "from votes where PostID = p.postid) as isliketotal, (select ifnull(sum(IsDislike),0) " +
				   "from votes where PostID = p.postid) as isdisliketotal, (select IF(islike = 1, 'Like', 'Dislike') from votes where postid = p.postid and userid = " + id + ") as yourvote from posts p inner join users u on u.id = p.UserID Where p.Solved = false and p.Archived = false order by isliketotal DESC, isdisliketotal ASC";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				sqlDa.Fill(sqlDt);

				
				List<posts> fposts = new List<posts>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					fposts.Add(new posts
					{
						postId = Convert.ToInt32(sqlDt.Rows[i]["PostId"]),
						userId = Convert.ToInt32(sqlDt.Rows[i]["UserID"]),
						firstName = sqlDt.Rows[i]["fname"].ToString(),
						lastName = sqlDt.Rows[i]["lname"].ToString(),
						post = sqlDt.Rows[i]["Post"].ToString(),
						department = sqlDt.Rows[i]["Department"].ToString(),
						date = Convert.ToDateTime(sqlDt.Rows[i]["DateTimes"]).ToString("MM/dd/yyyy hh:mm tt"),
						yourvote = sqlDt.Rows[i]["yourvote"].ToString(),
						likes = Convert.ToInt32(sqlDt.Rows[i]["isliketotal"]),
						dislikes = Convert.ToInt32(sqlDt.Rows[i]["isdisliketotal"]),
						isSolved = Convert.ToBoolean(sqlDt.Rows[i]["Solved"]),
						isArchived = Convert.ToBoolean(sqlDt.Rows[i]["Archived"])
					});
				}
				return fposts.ToArray();
			}
			else
			{
				return new posts[0];
			}
		}

		[WebMethod(EnableSession = true)]
		public void CreateArchivedPost(int postID)
		{
			string sqlconnectstring = getConString();
			string sqlselect = "UPDATE `440sum20221`.`posts` SET `Archived` = true WHERE `PostID` = " + postID + ";";

			MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
			MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);


			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		[WebMethod(EnableSession = true)]
		public posts[] GetArchived()
		{
			
			if (Session["id"] != null)
			{
				var id = Session["id"].ToString();
				DataTable sqlDt = new DataTable("posts");

				string sqlConnectString = getConString();
				string sqlSelect = "select posts.PostID, posts.UserID, posts.Post, posts.DateTimes, posts.Solved, posts.Department, users.fname, users.lname, users.email, " +
								   "posts.Archived, (select ifnull(sum(IsLike), 0) " +
								   "from votes where PostID = posts.postid) as isliketotal, (select ifnull(sum(IsDislike),0) " +
								   "from votes where PostID = posts.postid) as isdisliketotal, (select IF(islike = 1, 'Like', 'Dislike') from votes where postid = posts.postid and userid = " + id + ") as yourvote from posts inner join users on posts.UserID = users.id where posts.Archived = true order by posts.DateTimes desc";

				MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				sqlDa.Fill(sqlDt);

			
				List<posts> archivedPosts = new List<posts>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					archivedPosts.Add(new posts
					{
						postId = Convert.ToInt32(sqlDt.Rows[i]["postId"]),
						userId = Convert.ToInt32(sqlDt.Rows[i]["userId"]),
						post = sqlDt.Rows[i]["Post"].ToString(),
						department = sqlDt.Rows[i]["Department"].ToString(),
						date = Convert.ToDateTime(sqlDt.Rows[i]["DateTimes"]).ToString("MM/dd/yyyy hh:mm tt"),
						firstName = sqlDt.Rows[i]["fname"].ToString(),
						lastName = sqlDt.Rows[i]["lname"].ToString(),
						email = sqlDt.Rows[i]["email"].ToString(),
						likes = Convert.ToInt32(sqlDt.Rows[i]["isliketotal"]),
						dislikes = Convert.ToInt32(sqlDt.Rows[i]["isdisliketotal"]),
						yourvote = sqlDt.Rows[i]["yourvote"].ToString(),
						isSolved = Convert.ToBoolean(sqlDt.Rows[i]["Solved"]),
						isArchived = Convert.ToBoolean(sqlDt.Rows[i]["Archived"])
					});
				}
				return archivedPosts.ToArray();
			}
			else
			{
				return new posts[0];
			}
		}
        [WebMethod(EnableSession = true)]
		public void EditPost(string postID, string newPostText, string uid)
        {
			if (Session["id"] != null)
			{
				string sqlconnectstring = getConString();
				string sqlselect = "UPDATE posts SET Post=@postText WHERE PostID=@pid and UserID=@uidValue";

				MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
				MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@postText", HttpUtility.UrlDecode(newPostText));
				sqlCommand.Parameters.AddWithValue("@pid", HttpUtility.UrlDecode(postID));
				sqlCommand.Parameters.AddWithValue("@uidValue", HttpUtility.UrlDecode(uid));

			
				sqlConnection.Open();
			
				try
				{
					sqlCommand.ExecuteScalar();
					
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}
		[WebMethod(EnableSession = true)]
		public void EditComment(string commentID, string newCommentText, string uid)
		{
			if (Session["id"] != null)
			{
				string sqlconnectstring = getConString();
				string sqlselect = "UPDATE comments SET Comment=@commentText WHERE CommentID=@cid and UserID=@uidValue";

				MySqlConnection sqlConnection = new MySqlConnection(sqlconnectstring);
				MySqlCommand sqlCommand = new MySqlCommand(sqlselect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@commentText", HttpUtility.UrlDecode(newCommentText));
				sqlCommand.Parameters.AddWithValue("@cid", HttpUtility.UrlDecode(commentID));
				sqlCommand.Parameters.AddWithValue("@uidValue", HttpUtility.UrlDecode(uid));

			
				sqlConnection.Open();
				
				try
				{
					sqlCommand.ExecuteScalar();
				
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}
	}
}
