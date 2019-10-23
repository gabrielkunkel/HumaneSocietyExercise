using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////

        // Bonus!
        public static void AddAnimalsFromCSV()
        {
            var lines = File.ReadAllLines("animals.csv").Select(x => x.Split(',')
                            .Select(y => y.Trim()).ToArray());

            foreach (var item in lines)
            {
                Animal newAnimal = new Animal();

                newAnimal.Name = Clean.CleanString(item[0]);
                newAnimal.Weight = Int32.Parse(item[1]);
                newAnimal.Age = Int32.Parse(item[2]);
                newAnimal.Demeanor = Clean.CleanString(item[3]);
                newAnimal.KidFriendly = Clean.StringToBool(item[4]);
                newAnimal.PetFriendly = Clean.StringToBool(item[5]);
                newAnimal.Gender = Clean.CleanString(item[6]);
                newAnimal.AdoptionStatus = Clean.CleanString(item[7]);
                
                
                if (item[8] == "null")
                {

                }
                else
                {
                    newAnimal.CategoryId = Int32.Parse(item[8]);
                }
                if (item[8] == "null")
                {

                }
                else
                {
                    newAnimal.DietPlanId = Int32.Parse(item[8]);
                }
                if (item[8] == "null")
                {

                }
                else
                {
                    newAnimal.EmployeeId = Int32.Parse(item[8]);
                }        

                db.Animals.InsertOnSubmit(newAnimal);
                db.SubmitChanges();
            }


        }




        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "delete":
                    Employee employeeToDelete = db.Employees.Where(emp => emp.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();

                    db.Employees.DeleteOnSubmit(employeeToDelete);
                    db.SubmitChanges();
                    break;
                case "read":
                    var employeeToInfo = db.Employees.Where(emp => emp.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    UserInterface.DisplayEmployeeInfo(employeeToInfo);
                    break;
                case "update":
                    Employee employeeToUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault();
                    employeeToUpdate.FirstName = employee.FirstName;
                    employeeToUpdate.LastName = employee.LastName;
                    employeeToUpdate.EmployeeNumber = employee.EmployeeNumber;
                    employeeToUpdate.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                default:
                    break;
            }


        }


        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(e => e.AnimalId == id).FirstOrDefault();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            // find corresponding Animal from Db
            Animal animalFromDb = null;

            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == animalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No animals have an AnimalId that matches the Animal passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update AnimalFromDb information with the values on update

            foreach (KeyValuePair<int, string> item in updates)
            {
                switch (item.Key)
                {

                    case 1:
                        animalFromDb.CategoryId = GetCategoryId(item.Value);
                        break;
                    case 2:
                        animalFromDb.Name = item.Value;
                        break;
                    case 3:
                        animalFromDb.Age = Int32.Parse(item.Value);
                        break;
                    case 4:
                        animalFromDb.Demeanor = item.Value;
                        break;
                    case 5:
                        animalFromDb.KidFriendly = bool.Parse(item.Value);
                        break;
                    case 6:
                        animalFromDb.PetFriendly = bool.Parse(item.Value);
                        break;
                    case 7:
                        animalFromDb.Weight = Int32.Parse(item.Value);
                        break;
                    case 8:
                        animalFromDb.AnimalId = Int32.Parse(item.Value);
                        break;
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {

            IQueryable<Animal> animals = db.Animals;

            foreach (KeyValuePair<int, string> item in updates)
            {

                switch (item.Key)
                {

                    case 1:
                        animals = animals.Where(animal => animal.CategoryId == GetCategoryId(item.Value));
                        break;
                    case 2:
                        animals = animals.Where(animal => animal.Name == item.Value);

                        break;
                    case 3:
                        animals = animals.Where(animal => animal.Age == Int32.Parse(item.Value));

                        break;
                    case 4:
                        animals = animals.Where(animal => animal.Demeanor == item.Value);

                        break;
                    case 5:
                        animals = animals.Where(animal => animal.KidFriendly == bool.Parse(item.Value));

                        break;
                    case 6:
                        animals = animals.Where(animal => animal.PetFriendly == bool.Parse(item.Value));

                        break;
                    case 7:
                        animals = animals.Where(animal => animal.Weight == Int32.Parse(item.Value));

                        break;
                    case 8:
                        animals = animals.Where(animal => animal.AnimalId == Int32.Parse(item.Value));

                        break;
                }

            }

            return animals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(category => category.Name == categoryName).Select(category => category.CategoryId).FirstOrDefault();
        }

        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(room => room.AnimalId == animalId).FirstOrDefault();
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(plan => plan.Name == dietPlanName).Select(plan => plan.DietPlanId).FirstOrDefault();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.ApprovalStatus = "Pending";
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(adoption => adoption.ApprovalStatus == "Pending");
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            adoption.ApprovalStatus = isAdopted ? "Approved" : "Denied";
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).SingleOrDefault();

            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var relevantShots = db.AnimalShots.Where(shot => shot.AnimalId == animal.AnimalId);
            return relevantShots;           
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot newAnimalShot = new AnimalShot();
            
            var newShotID = db.Shots.Where(shot => shot.Name == shotName).Select(shotx => shotx.ShotId).SingleOrDefault();
           
            newAnimalShot.AnimalId = animal.AnimalId;
            newAnimalShot.ShotId = newShotID;
            newAnimalShot.DateReceived = DateTime.Now;

            db.AnimalShots.InsertOnSubmit(newAnimalShot);
            db.SubmitChanges();
        }
    }
}