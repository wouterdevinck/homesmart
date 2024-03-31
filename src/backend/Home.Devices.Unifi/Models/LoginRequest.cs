using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class LoginRequest {

        public LoginRequest(string username, string password) { 
            Username = username;
            Password = password;
        }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("token")]
        public string Token => string.Empty;

        [JsonProperty("rememberMe")]
        public bool RememberMe => false;

    }

}