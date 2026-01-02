using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyConnectApp.Classes
{
    internal static class FirestoreHelper
    {

        static string fireconfig = @"
        {
          ""type"": ""service_account"",
          ""project_id"": ""facultyconnectdb"",
          ""private_key_id"": ""ebf14846f771c9b794f2060a50bc6df383d07aa5"",
          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDLrxWwjnFJBbZx\nKmw6l/jM7swp+4BtUy0rL6DkV8JKOxs/2ufNYIwLHkJHpj4bfUPYlCppq6MQwxwq\nij9ZOBvf7ihArEoCm9obJAx1X0iGTWM7jovbXMNuN3oRWA1thUbpbdn/y+iJul9k\nWMpWkqGKuVxcJEVic+q2I9lM1Q3aiVKkVhJ4V9+ARxCbJDpB3+Si/FTYx48IvHKt\nRB/ZRkPApp/R8ucOiw9ymnwGMytr+kB27ErcyAWDVLCgtZebIPd4YcaS6WP8omY7\n8mcObUqucOZ9A9C25dugSaLJEzj2XMLJSW3KaULhqWisWgRxb4CS/ZOEoWbDdti9\nyMRB/6VJAgMBAAECggEABl//yepAE7OO8k5he46ANNAoZKEovn3kVlC77zWrdjW7\ndzrM4fCABZo6zmxeudnIduCBTil3Sw9ORABryz6JNHpV48X4st70UJXm4WwRadRa\n0yAAUttIXxxeUNQR5zACaDczg+ZFYZKAnpQ0W166GWH5PkWLhJFhTYuERzuNCX5P\n5iDHW5TT4M9hU0BNKww3wUv5o6lEhbeMDucLtvI0HEcU2G26bHFm9+H50nhNdk8Y\nzoWLPBP6Znn1OIO2MUO5NOBZ5ORcl34wQQa3VwWWxtc5xkSJblTzQu1dT3GmBaQM\nr6+0n5rPAachUjmXJmlc6Rm7CSIa0CxTtuUN4uawUQKBgQD0lkRhWGUwB4FIF9PY\nRHachd4nMtDgBDPQ9NlUVHNamvblODgo4tdJnd1SCORuwKQjKG+kumZxuKw0ilLG\nO/Yf2ljheNxnuakD6kCo3RdqRxIxm0bgsw1lDRp2ROcJBOwiHz0ch0NzKjV4tz12\n4EB6+UJ+cXp3gYrG9N7DFINVeQKBgQDVMDT7bUsRUnTh9jzoYay4tilvLfxxgFy/\n2J044TnOEnqf9WzyGWYFL4vd3ubweFNZYUSQbIUI7nf+jvH2rPeqZk67Gxxf8Pv2\nt09HTBFuTLanQhRWmueu0eH/3Yr/AnjW52nV2AxFEZ9nWzJwyf59bC4qkDmdeyih\nGFg3+5nqUQKBgArGP9bl9PcrrXdGjW9+fJcikom5hFgJ91piHtzHJ5m7L9sjId4z\nN/anPKOrfpyeYdymoFxqVa72yqc53LGc3JfEn5u3HkZ3eEmS3SxTKP/mh5el1nZ5\njMKB1EOXf3H3RIuwQpnqH3+IQXjC4bNF57FDH7nAN/vhugJPsSx4z9xJAoGAND7d\nLSTqubMIe/v5j0Woq5fQ7bNKY5J+qHFwjmj73pm+vYbLDUXWL70oPNaMqDAE1Sm2\nQOKnc7nlZFgpyjc3duYds1MAkC6hwSPJZQKqXuqj9LeH/nV2A+zhwu/LSZWUga4y\nzLpmv2KDOzQpV2TZXmltova3d/WfzmMF86pQXRECgYBK60uuU/ktLbmfUNBWiLmU\noFpG8NZi30yPd8qSAzd2UcCS/9wq/3XSyCG++lDMIbzuTJ9NynU/lUmfV3U1QePp\nx+zyo3YiT3aAGTZlyns2zlDmCfD/HdMne9Dt7yyvmjerWv/8T3xbyTHNScsGF7VJ\nfAH6fpXSnuVPz+vdURGlxw==\n-----END PRIVATE KEY-----\n"",
          ""client_email"": ""firebase-adminsdk-fbsvc@facultyconnectdb.iam.gserviceaccount.com"",
          ""client_id"": ""114339929393853205004"",
          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
          ""token_uri"": ""https://oauth2.googleapis.com/token"",
          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-fbsvc%40facultyconnectdb.iam.gserviceaccount.com"",
          ""universe_domain"": ""googleapis.com""
        }";

        static string filepath = "";
        static FirestoreDb database;

        public static void SetEnvireomentVariable()
        {
            filepath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(filepath, fireconfig);
            File.SetAttributes(filepath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            database = FirestoreDb.Create("facultyconnectdb");
            File.Delete(filepath);

        }

        public static FirestoreDb Database
        {
            get
            {
                if (database == null)
                    database = FirestoreDb.Create("facultyconnectdb");
                return database;
            }
        }

    }
}
