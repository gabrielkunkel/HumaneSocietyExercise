﻿using System;
using System.Collections.Generic;
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

    // TODO: Allow any of the CRUD operations to occur here
    internal static void RunEmployeeQueries(Employee employee, string crudOperation)
    {
      throw new NotImplementedException();
    }

    // TODO: Animal CRUD Operations
    internal static void AddAnimal(Animal animal)
    {
      throw new NotImplementedException();
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
        throw new NotImplementedException();
      }

      // TODO: Animal Multi-Trait Search
      internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
      {
            //throw new NotImplementedException();

            IQueryable<Animal> animals = db.Animals;

            foreach(KeyValuePair<int, string> item in updates)
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
        throw new NotImplementedException();
      }

      internal static int GetDietPlanId(string dietPlanName)
      {
        throw new NotImplementedException();
      }

      // TODO: Adoption CRUD Operations
      internal static void Adopt(Animal animal, Client client)
      {
        throw new NotImplementedException();
      }

      internal static IQueryable<Adoption> GetPendingAdoptions()
      {
        throw new NotImplementedException();
      }

      internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
      {
        throw new NotImplementedException();
      }

      internal static void RemoveAdoption(int animalId, int clientId)
      {
        throw new NotImplementedException();
      }

      // TODO: Shots Stuff
      internal static IQueryable<AnimalShot> GetShots(Animal animal)
      {
        throw new NotImplementedException();
      }

      internal static void UpdateShot(string shotName, Animal animal)
      {
        throw new NotImplementedException();
      }
    }
  }