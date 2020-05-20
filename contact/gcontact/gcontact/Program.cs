using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using System;
using System.IO;
using System.Threading;

namespace gcontact
{
    class Program
    {
        static void Main(string[] args)            
        {
            auth();
        }


        static void auth()
        {
        //string[] Scopes = new string[] { "https://www.googleapis.com/auth/contacts.readonly" };     // view your basic profile info.
        
            // Requesting access to Contacts API and Groups Provisioning API
        string[] Scopes = new string[] { 
            "https://www.googleapis.com/auth/contacts.readonly" ,
            "https://www.google.com/m8/feeds/",
            "https://apps-apis.google.com/a/feeds/groups/"
        };     // view your basic profile info.


            try
            {
                //string clientId = "xxxxxx.apps.googleusercontent.com";
                //string clientSecret = "xxxxx";
                ////// Use the current Google .net client library to get the Oauth2 stuff.
                ////UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                ////                                                                             , Scopes
                ////                                                                             , "test"
                ////                                                                             , CancellationToken.None
                ////                                                                             , new FileDataStore("test")).Result;

                UserCredential credential;
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                ////// Create Gmail API service.
                ////var service = new GmailService(new BaseClientService.Initializer()
                ////{
                ////    HttpClientInitializer = credential,
                ////    ApplicationName = ApplicationName,
                ////});

                

                // Translate the Oauth permissions to something the old client libray can read
                OAuth2Parameters parameters = new OAuth2Parameters();
                parameters.AccessToken = credential.Token.AccessToken;
                parameters.RefreshToken = credential.Token.RefreshToken;
                RunContactsSample(parameters);
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                Console.ReadLine();

            }
            Console.ReadLine();


        }

        /// <summary> 
        /// Send authorized queries to a Request-based library 
        /// </summary> 
        /// <param name="service"></param> 
        static void RunContactsSample(OAuth2Parameters parameters)
        {
            try
            {
                RequestSettings settings = new RequestSettings("Google contacts tutorial", parameters);
                ContactsRequest cr = new ContactsRequest(settings);

                //ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
                //query.StartDate = new DateTime(2001, 1, 1);
                //Feed<Contact> feed = cr.Get<Contact>(query);
                //foreach (Contact contact in feed.Entries)
                //{
                //    Console.WriteLine(contact.Name.FullName);
                //    //Console.WriteLine("Updated on: " + contact.Updated.ToString());
                //}

                GroupsQuery query = new GroupsQuery(GroupsQuery.CreateGroupsUri("default")); 
                query.StartDate = new DateTime(2001, 1, 1); 
                Feed<Group> feed = cr.Get<Group>(query);

                foreach (Group group in feed.Entries)
                {
                    Console.WriteLine(group.Title);
                    Console.WriteLine("Updated on: " + group.Updated.ToString());
                }

                ////string applicationName = "Test-OAuth2";
                ////GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory("apps", applicationName, parameters);
                ////GroupsService service = new GroupsService(domain, applicationName);
                ////service.RequestFactory = requestFactory;
                ////GroupFeed feed = service.RetrieveAllGroups();
                ////foreach (GroupEntry group in feed.Entries)
                ////{
                ////    Console.WriteLine(group.GroupName);
                ////}

                //Feed<Contact> f = cr.GetContacts();
                //foreach (Contact c in f.Entries)
                //{
                //    Console.WriteLine(c.Name.FullName);
                //}
            }
            catch (Exception a)
            {
                Console.WriteLine("A Google Apps error occurred.");
                Console.WriteLine();
            }
        }

    }
}
