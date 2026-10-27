using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;

public class MongoDBManager : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> usersCollection;
    private IMongoCollection<BsonDocument> userStatsCollection;

    private string connectionString = "mongodb+srv://apuig2dam:Campalans1234!@unitycluster.kd6ad.mongodb.net/game_stats";
    public static MongoDBManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Evita duplicats
            return;
        }
    }



    void Start()
    {
        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("game_stats");

            userStatsCollection = database.GetCollection<BsonDocument>("user_stats");
            usersCollection = database.GetCollection<BsonDocument>("users");

            Debug.Log("MongoDB connected successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"MongoDB connection error: {ex.Message}");
        }
    }

    public void InitializeUserStats(string username)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        if (userStatsCollection.Find(filter).FirstOrDefault() == null)
        {
            var document = new BsonDocument
            {
                { "username", username },
                { "totalDuration", 0 },
                { "enemiesDefeated", new BsonDocument
                    {
                        { "Fire", 0 }, { "Ice", 0 }, { "Lightning", 0 },
                        { "MeleeFire", 0 }, { "MeleeIce", 0 }, { "MeleeLightning", 0 }
                    }
                },
                { "weaponsUsed", new BsonDocument
                    {
                        { "ProjectileFire", 0 }, { "ProjectileIce", 0 }, { "ProjectileLightning", 0 },
                        { "Ability2Fire", 0 }, { "Ability2Ice", 0 }, { "Ability2Lightning", 0 } , {"Sword", 0 }
                    }
                },
            };
            userStatsCollection.InsertOne(document);
            Debug.Log($"Initialized stats for user: {username}");
        }
    }

    // Actualitza les estadístiques de l'usuari
    public void UpdateUserStats(string username, string enemyType, string weaponType)
    {
        try
        {
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);

           
            if (userStatsCollection.Find(filter).FirstOrDefault() == null)
            {
                InitializeUserStats(username);
            }

            // Actualitza enemics eliminats
            var updateEnemy = Builders<BsonDocument>.Update.Inc($"enemiesDefeated.{enemyType}", 1);
            var resultEnemy = userStatsCollection.UpdateOne(filter, updateEnemy);
            Debug.Log($"Enemics actualitzats: {resultEnemy.ModifiedCount}, tipus: {enemyType}");

            // Actualitza armes utilitzades
            if (!string.IsNullOrEmpty(weaponType))
            {
                var updateWeapon = Builders<BsonDocument>.Update.Inc($"weaponsUsed.{weaponType}", 1);
                var resultWeapon = userStatsCollection.UpdateOne(filter, updateWeapon);
                Debug.Log($"Armes actualitzades: {resultWeapon.ModifiedCount}, tipus: {weaponType}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al actualitzar estadístiques: {ex.Message}");
        }
    }

    // Actualitza la durada de la sessió
    public void UpdateSessionDuration(string username, double duration)
    {
        try
        {
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);

            
            var userStats = userStatsCollection.Find(filter).FirstOrDefault();
            double currentDuration = 0;

            Debug.Log($"Updating duration for user {username}: +{duration}s");

            if (userStats != null && userStats.Contains("totalDuration"))
            {
               
                if (userStats["totalDuration"].IsDouble)
                    currentDuration = userStats["totalDuration"].AsDouble;
                else if (userStats["totalDuration"].IsInt32)
                    currentDuration = userStats["totalDuration"].AsInt32;
                else if (userStats["totalDuration"].IsInt64)
                    currentDuration = userStats["totalDuration"].AsInt64;
                else if (userStats["totalDuration"].IsString)
                    double.TryParse(userStats["totalDuration"].AsString, out currentDuration);

                Debug.Log($"Found existing duration: {currentDuration}s");
            }
            else
            {
                Debug.Log($"No existing duration found, initializing stats for {username}");
                InitializeUserStats(username);
            }

            double newDuration = currentDuration + duration;

            
            var update = Builders<BsonDocument>.Update.Set("totalDuration", newDuration);
            var result = userStatsCollection.UpdateOne(filter, update);

            Debug.Log($"Updated session duration: {currentDuration}s + {duration}s = {newDuration}s");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating session duration: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    // Obtenir estadístiques de l'usuari
    public BsonDocument GetUserStats(string username)
    {
        try
        {
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var result = userStatsCollection.Find(filter).FirstOrDefault();

            if (result == null)
            {
                Debug.LogWarning($"No stats found for user: {username}");
                InitializeUserStats(username);
                result = userStatsCollection.Find(filter).FirstOrDefault();
            }

            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting user stats: {ex.Message}");
            return null;
        }
    }

    public void SaveCurrentGameStats(
        double duration,
        int fireEnemies,
        int iceEnemies,
        int lightningEnemies,
        int meleeFireEnemies,
        int meleeIceEnemies,
        int meleeLightningEnemies,
        int projectileFire,
        int projectileIce,
        int projectileLightning,
        int ability2Fire,
        int ability2Ice,
        int ability2Lightning,
        int sword)
    {
        string username = PlayerPrefs.GetString("LoggedInUser", "Offline");
        if (username == "Offline") return; 

        var filter = Builders<BsonDocument>.Filter.Eq("username", username);

        // Crea el document si no existeix
        if (userStatsCollection.Find(filter).FirstOrDefault() == null)
            InitializeUserStats(username);

        var update = Builders<BsonDocument>.Update
            // Enemics
            .Inc("enemiesDefeated.Fire", fireEnemies)
            .Inc("enemiesDefeated.Ice", iceEnemies)
            .Inc("enemiesDefeated.Lightning", lightningEnemies)
            .Inc("enemiesDefeated.MeleeFire", meleeFireEnemies)
            .Inc("enemiesDefeated.MeleeIce", meleeIceEnemies)
            .Inc("enemiesDefeated.MeleeLightning", meleeLightningEnemies)
            // Armes
            .Inc("weaponsUsed.ProjectileFire", projectileFire)
            .Inc("weaponsUsed.ProjectileIce", projectileIce)
            .Inc("weaponsUsed.ProjectileLightning", projectileLightning)
            .Inc("weaponsUsed.Ability2Fire", ability2Fire)
            .Inc("weaponsUsed.Ability2Ice", ability2Ice)
            .Inc("weaponsUsed.Ability2Lightning", ability2Lightning)
            .Inc("weaponsUsed.Sword", sword);

        userStatsCollection.UpdateOne(filter, update);

        // Actualitza la durada total
        UpdateSessionDuration(username, duration);

        Debug.Log($"[MongoDB] Stats de la partida afegides per {username}");
    }







    public bool RegisterUser(string username, string password)
    {
        try
        {
            // Verifica si l'usuari ja existeix
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            if (usersCollection.Find(filter).FirstOrDefault() != null)
            {
                Debug.LogError("L'usuari ja existeix!");
                return false;
            }

            // Xifra la contrasenya
            string passwordHash = HashPassword(password);

            // Crea el document
            var document = new BsonDocument
            {
                { "username", username },
                { "passwordHash", passwordHash },
            };

            usersCollection.InsertOne(document);

            
            InitializeUserStats(username);

            Debug.Log($"User registered successfully: {username}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error registering user: {ex.Message}");
            return false;
        }
    }

    // Autentica un usuari
    public bool AuthenticateUser(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogError("Username or password is empty");
                return false;
            }

            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var user = usersCollection.Find(filter).FirstOrDefault();

            if (user == null)
            {
                Debug.LogError($"User not found: {username}");
                return false;
            }

            string storedHash = user["passwordHash"].AsString;
            string inputHash = HashPassword(password);

            bool result = (storedHash == inputHash);
            Debug.Log($"Authentication result for {username}: {result}");
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Authentication error: {ex.Message}");
            return false;
        }
    }



    // Xifratge de contrasenyes (SHA-256)
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes) builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
}