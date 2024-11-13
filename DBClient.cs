using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Databaseopgave
{
    class DBClient
    {
       
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HotelDB;Integrated Security=True;";

        private int GetMaxFacilityId(SqlConnection connection)
        {
            Console.WriteLine("Calling -> GetMaxFacilityId");

          
            string queryStringMaxFacilityId = "SELECT  MAX(Facility_id)  FROM Facility";
            Console.WriteLine($"SQL applied: {queryStringMaxFacilityId}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(queryStringMaxFacilityId, connection);
            SqlDataReader reader = command.ExecuteReader();

          
            int MaxFacilityId = 0;

            //Is there any rows in the query
            if  (reader.Read())
            {
               
                MaxFacilityId = reader.GetInt32(0); 
            }

          
            reader.Close();
            
            Console.WriteLine($"Max FacilityId#: {MaxFacilityId}");
            Console.WriteLine();

            
            return MaxFacilityId;
        }

        private int DeleteFacility(SqlConnection connection, int FacilityId)
        {
            Console.WriteLine("Calling -> DeleteFacility");

      
            string deleteCommandString = $"DELETE FROM Facility  WHERE Facility_id = {FacilityId}";
            Console.WriteLine($"SQL applied: {deleteCommandString}");

            
            SqlCommand command = new SqlCommand(deleteCommandString, connection);
            Console.WriteLine($"Deleting Facility #{FacilityId}");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            
            return numberOfRowsAffected;
        }

        private int UpdateFacility(SqlConnection connection, Facility facility)
        {
            Console.WriteLine("Calling -> UpdateFacility");

  
            string updateCommandString = $"UPDATE Facility SET Name='{facility.Facility_Name}' WHERE Facility_id = {facility.Facility_id}";
            Console.WriteLine($"SQL applied: {updateCommandString}");

          
            SqlCommand command = new SqlCommand(updateCommandString, connection);

            Console.WriteLine($"Updating Facility #{facility.Facility_id}"); 
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            
            return numberOfRowsAffected;
        }

        private int InsertFacility(SqlConnection connection, Facility facility)
        {
            Console.WriteLine("Calling -> Insertfacility");

       
            string insertCommandString = $"INSERT INTO Facility VALUES({facility.Facility_id}, '{facility.Facility_Name}')";
            Console.WriteLine($"SQL applied: {insertCommandString}");

            
            SqlCommand command = new SqlCommand(insertCommandString, connection);
            
            Console.WriteLine($"Creating facility #{facility.Facility_id}");

            int numberOfRowsAffected = command.ExecuteNonQuery();
            
            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            
            return numberOfRowsAffected;
        }

        private List<Facility> ListAllFacility(SqlConnection connection)
        {
            Console.WriteLine("Calling -> ListAllFacility");

            
            string queryStringAllFacility = "SELECT * FROM Facility";
            Console.WriteLine($"SQL applied: {queryStringAllFacility}");

            
            SqlCommand command = new SqlCommand(queryStringAllFacility, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("Listing all facilities:");

            
            if (!reader.HasRows)
            {
               
                Console.WriteLine("No facility in database");
                reader.Close();
                
                return null;
            }

            //Create list of found facilities
            List<Facility> ListOfFacilities = new List<Facility>();
            while (reader.Read())
            {
               
                Facility nextFacility = new Facility()
                {
                    Facility_id = reader.GetInt32(0), 
                    Facility_Name = reader.GetString(1),   
                    
                };

                
                ListOfFacilities.Add(nextFacility);

                Console.WriteLine(nextFacility);
            }

           
            reader.Close();
            Console.WriteLine();

            
            return ListOfFacilities;
        }

        private Facility GetFacility(SqlConnection connection, int facilityId)
        {
            Console.WriteLine("Calling -> GetFacility");

         
            string queryStringOneFacility = $"SELECT * FROM Facility WHERE Facility_id = {facilityId}";
            Console.WriteLine($"SQL applied: {queryStringOneFacility}");

   
            SqlCommand command = new SqlCommand(queryStringOneFacility, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine($"Finding facility#: {facilityId}");

            
            if (!reader.HasRows)
            {
               
                Console.WriteLine("No facility in database");
                reader.Close();

                
                return null;
            }

           
            Facility facility = null; 
            if (reader.Read())
            {
                facility = new Facility()
                {
                    //Reading int from 1st column
                    Facility_id = reader.GetInt32(0),
                    //Reading string from 2nd column
                    Facility_Name = reader.GetString(1),   
                };
                   
                Console.WriteLine(facility);
            }

            reader.Close();
            Console.WriteLine();

            
            return facility;
        }
        public void Start()
        {
           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               
                connection.Open();

               
                ListAllFacility(connection);

                //Create a new facility with primary key equal to current max primary key plus 1
                Facility newFacility = new Facility()
                {
                    Facility_id = GetMaxFacilityId(connection) + 1,
                    Facility_Name = "New Facility",
                   
                };

                //Insert the facility into the database
                InsertFacility(connection, newFacility);

                //List all facilties including the the new inserted facility 
                ListAllFacility(connection);

                //Get the newly inserted facility from the database in order to update it 
                Facility facilityToBeUpdated = GetFacility(connection, newFacility.Facility_id);

                facilityToBeUpdated.Facility_Name += "(updated)";
              
                //Update the facility in the database 
                UpdateFacility(connection, facilityToBeUpdated);

                //List all facilites including the updated one
                ListAllFacility(connection);

                //Get the updated facility in order to delete it
                Facility facilityToBeDeleted = GetFacility(connection, facilityToBeUpdated.Facility_id);

                //Delete the facility
                DeleteFacility(connection, facilityToBeDeleted.Facility_id);

                //List all facilities - now without the deleted facility
                ListAllFacility(connection);
            }
        }
    }
}
