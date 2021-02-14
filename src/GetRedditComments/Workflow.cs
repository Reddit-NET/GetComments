using Reddit;
using Reddit.Controllers;
using Things = Reddit.Things;
using System;
using System.Collections.Generic;

namespace GetRedditComments
{
    public class Workflow
    {
        private RedditClient Reddit;
        private Post Post;
        private int NumComments = 0;
        private HashSet<string> CommentIds;

        public Workflow(string[] args)
        {
            string appId = args[0];
            string refreshToken = args[1];
            string accessToken = (args.Length > 2 ? args[2] : null);

            // Initialize the API library instance.  --Kris
            Reddit = new RedditClient(appId: appId, refreshToken: refreshToken, accessToken: accessToken);
            Post = Reddit.Subreddit("news").Post("t3_2lt3d0").About();
            CommentIds = new HashSet<string>();
        }

        /* Note that it's not possible to retrieve more than 500 comments on a post (1,500 if the active user has Reddit Gold).  --Kris */
        // See: https://www.reddit.com/r/help/comments/2blbdm/is_there_any_way_to_see_more_than_500_comments_in/
        public void Run()
        {
            Console.WriteLine(Post.Title);
            Console.WriteLine("There are " + Post.Listing.NumComments + " comments:");

            IterateComments(Post.Comments.GetNew(limit: 500));

            Console.WriteLine("Total Comments Iterated: " + NumComments);
            Console.WriteLine("Total Unique Comments: " + CommentIds.Count);
        }

        private void IterateComments(IList<Comment> comments, int depth = 0)
        {
            foreach (Comment comment in comments)
            {
                ShowComment(comment, depth);
                IterateComments(comment.Replies, (depth + 1));
                IterateComments(GetMoreChildren(comment), depth);
            }
        }

        private IList<Comment> GetMoreChildren(Comment comment)
        {
            List<Comment> res = new List<Comment>();
            if (comment.More == null)
            {
                return res;
            }

            foreach (Things.More more in comment.More)
            {
                foreach (string id in more.Children)
                {
                    if (!CommentIds.Contains(id))
                    {
                        res.Add(Post.Comment("t1_" + id).About());
                    }
                }
            }

            return res;
        }

        private void ShowComment(Comment comment, int depth = 0)
        {
            if (comment == null || string.IsNullOrWhiteSpace(comment.Author))
            {
                return;
            }

            NumComments++;
            if (!CommentIds.Contains(comment.Id))
            {
                CommentIds.Add(comment.Id);
            }

            if (depth.Equals(0))
            {
                Console.WriteLine("---------------------");
            }
            else
            {
                for (int i = 1; i <= depth; i++)
                {
                    Console.Write("> ");
                }
            }

            Console.WriteLine("[" + comment.Author + "] " + comment.Body);
        }
    }
}
