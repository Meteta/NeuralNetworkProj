using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameList 
{
    // First Names
    public string FirstNames(ActorBehaviour.Species species, System.Random random) {
        List<string> firstNames = new List<string>();

        if(species == ActorBehaviour.Species.Human){
            string[] names = {
                "John", "Dave"
            };
            firstNames.AddRange(names);
        } else if(species == ActorBehaviour.Species.Elf){
            string[] names = {
                "Ysla", "Virty"
            };
            firstNames.AddRange(names);
        } else if(species == ActorBehaviour.Species.Dwarf){
            string[] names = {
                "Brim", "Yodin"
            };
            firstNames.AddRange(names);
        } else if(species == ActorBehaviour.Species.Ork){
            string[] names = {
                "Gorruk", "Norr"
            };
            firstNames.AddRange(names);
        } else {
            firstNames.Add("Placeholder");
        }
        string firstName = firstNames[random.Next(firstNames.Count)];
        return firstName;
    }

    // Last Names
    public string LastNames(ActorBehaviour.Species species, System.Random random) {
        List<string> lastNames = new List<string>();

        if(species == ActorBehaviour.Species.Human){
            string[] names = {
                "Man", "Dude"
            };
            lastNames.AddRange(names);
        } else if(species == ActorBehaviour.Species.Elf){
            string[] names = {
                "Moonlight", "Dawnshine"
            };
            lastNames.AddRange(names);
        } else if(species == ActorBehaviour.Species.Dwarf){
            string[] names = {
                "Stoneskin", "Coalbeard"
            };
            lastNames.AddRange(names);
        } else if(species == ActorBehaviour.Species.Ork){
            string[] names = {
                "Teethnasher", "Bladekiller"
            };
            lastNames.AddRange(names);
        } else {
            lastNames.Add("Placeholder");
        }
        string lastName = lastNames[random.Next(lastNames.Count)];
        return lastName;
    }
}
