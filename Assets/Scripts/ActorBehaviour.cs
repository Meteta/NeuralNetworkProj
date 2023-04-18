using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class ActorBehaviour : MonoBehaviour
{
    // MUST HAVES
    // Need Energy To Survive.
    // Have a Maturity they need to reach to reproduce.
    // Traits with values and a maximum number of points that can be spent on them total. 500 Points, 100 in strength 200 in dexterity, etc.

    // SHOULD HAVES
    // Can reproduce with another creature
    // Ability to grab and move objects
    // Attack other actors

    // COULD HAVES
    // Abilities
    // Gear

    // Behaviours
    public NerualNetwork nerualNetwork;
    public bool lookingForMate {get; private set;} = false; 
    public bool aggressive {get; private set;} = false;
    public bool recentBirth {get; private set;}
    public float lastBirth {get; private set;}
    public int birthCount {get; private set;}

    // Evolution
    public int evolutionCount {get; private set;}
    public float lastEvolution {get; private set;}
    public float lastEvolutionAttempt {get; private set;}

    // Attributes (these effect different things)
    private float strength; //Effects Damage, and move speed while grabbing
    private float dexterity; //Effects move speed, and attack speed
    private float constitution; //Max Health and Energy
    private float wisdom; //Perception FOV
    private float intelligence; //Perception Distance
    private float charisma; // Nothing atm

    // Status Variables
    public enum Species {
        Primal,
        Human,
        Elf,
        Dwarf,
        Ork
    }
    public Species race = Species.Primal; 

    private float maxHealth;
    private float maxEnergy;
    public float energy;
    public bool hungry;
    public float health;
    public float maturity {get; private set;} = 0;
    public float matureAge = 18f; // Default to 18

    // Movement Variables
    public float movementSpeed;
    public float rotationSpeed;
    private bool lockMovement;

    // Sight Variables
    private GameObject focus;
    public bool canSeeFocus {get; private set;}
    [Range(1, 360)] public float fieldOfView;
    public float radius;
    public LayerMask obstructionMask;
    public LayerMask actorMask;
    public LayerMask defaultMask;
    public int focusID;

    // Base Combat variables
    public float attackDamage {get; private set;}
    public float attackSpeed {get; private set;}
    public float attackForce {get; private set;}
    private float lastAttack;
    private bool canAttack;

    // Misc Variables
    public Spawner spawner;
    static System.Random random = new System.Random();
    public GameObject drop1;
    public Transform castPoint;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer rend;
    public ParticleSystem damageEffect;
    public ParticleSystem deathEffect;
    private NameList nameList = new NameList();
    public string firstName;
    public string lastName;
    public string fullName = "Place Holder";
    public Canvas canvas;
    public TextMeshProUGUI identity;

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponentInChildren<SpriteRenderer>();
        InitializeActor();
        InvokeRepeating("LifeCycle",0f,0.1f);
        InvokeRepeating("FieldOfView",0f,0.2f); // 5 times per second
        rigidBody2D = GetComponent<Rigidbody2D>();
        InvokeRepeating("ReturnToWhite",0f,1f);
    }

    void ReturnToWhite(){
        rend.color = Color.white;
    }

    void Update()
    {
        // Handle Information Display
        canvas.transform.rotation = Camera.main.transform.rotation;

        // These If statements will have to be changed when NN is working
        if(Input.GetKey(KeyCode.W)) {
            MoveForward();
        }
        if(Input.GetKey(KeyCode.A)) {
            TurnLeft();
        }
        if(Input.GetKey(KeyCode.S)) {
            MoveBackward();
        }
        if(Input.GetKey(KeyCode.D)) {
            TurnRight();
        }

        if (Vector2.Distance(transform.position, Vector2.zero) > 5f)
        {
            lockMovement = true;
            MoveForward();
            TurnTowardsPosition(Vector3.zero);
        } else {
            lockMovement = false;
        }


        // SIGHT MOVEMENT CONTROLS
        // Remove when figure out Neural
        if(canSeeFocus && !lockMovement){
            if(focus.CompareTag("Organic") || focus.CompareTag("Meat")){
                if (hungry){
                    TurnTowards(focus.transform);
                    MoveForward();
                } else {
                    Unfocus();
                }
            } else if(focus.CompareTag("DefaultActor")){
                ActorBehaviour focusScript = focus.gameObject.GetComponent<ActorBehaviour>();
                if(focusScript.lookingForMate && lookingForMate && focusScript.race == race){
                    TurnTowards(focus.transform);
                    MoveForward();
                } else if (aggressive && focusScript.race != race){
                    TurnTowards(focus.transform);
                    MoveForward();
                    if(!canAttack){
                        Unfocus();
                    }
                } else if(focusScript.aggressive && !aggressive){ // If the other Actor is aggressive, we'd like to move away from them
                    TurnTowards(focus.transform);
                    MoveBackward();
                } else { //If the other Actor is not aggressive, we dont care :) and we can focus on something else.
                    Unfocus();
                }
            } 
        } else if (!lockMovement){ // If not focusing on anything, wander around.
            float chancifier = UnityEngine.Random.Range(0f,100f);
            if(chancifier <= 33f){
               MoveForward();
            } else if(chancifier >= 66)
            {
                TurnRight();
            } else
            {
                TurnLeft();
            }
        }
    }
    static T RandomEnumValue<T> ()
    {
        var v = Enum.GetValues (typeof (T));
        return (T) v.GetValue (random.Next(v.Length));
    }
    public void InitializeActor() {
        maturity = 0f;

        firstName = nameList.FirstNames(race, random);
        lastName = nameList.LastNames(race, random);
        fullName = $"{firstName} {lastName}"; 
        identity.text = $"{fullName} | {race}";
        
        strength = UnityEngine.Random.Range(1f,5f); //Effects Damage, and move speed while grabbing
        dexterity = UnityEngine.Random.Range(1f,5f); //Effects move speed, and attack speed
        constitution = UnityEngine.Random.Range(1f,5f); //Max Health and Energy
        wisdom = UnityEngine.Random.Range(1f,5f); //Perception field of view
        intelligence = UnityEngine.Random.Range(1f,5f); //Perception Distance
        charisma = UnityEngine.Random.Range(1f,5f);
        UpdateActor();
    }

    public void UpdateActor(){
         // Initialize Perception Stats
        fieldOfView = Mathf.Min((wisdom * 2) + 45f, 360);
        radius = intelligence * 0.5f;

        // Initialize Movement Stats
        movementSpeed = dexterity * 0.25f;
        rotationSpeed = 0.25f + (dexterity * 0.01f);

        // Initialize Combat Stats
        attackDamage = strength + maturity * 0.25f; // Increases as stats Increase
        attackSpeed = Mathf.Max(100f / (dexterity + maturity), 25f); // Decreases as stats Increase
        attackForce = (dexterity + strength) * 0.05f;

        // Initialize health and energy values on spawn
        maxHealth = 10f + (constitution + health) * 0.5f;
        maxEnergy = 10f + (constitution + dexterity) * 0.5f;
        energy = maxEnergy;
        health = maxHealth;
    }

    public void LifeCycle(){
        if(race == Species.Primal) { // Primals are an initialization Enum that allows spawned Actors to be randomly assigned a race.
            race = RandomEnumValue<Species>();
            identity.text = $"{fullName} | {race}";
        } 
        float currScale = (maxHealth + maxEnergy + maturity) * 0.01f;
        transform.localScale = new Vector3(currScale, currScale, currScale);
        ChangeEnergy(-0.0001f);
        maturity += 0.01f;

        if(energy <= maxEnergy * 0.8f){ //If the creature has only 80% of total potential energy, it is hungry
            hungry = true;
        } else if (energy > maxEnergy * 0.9f) { //If the creature has more than 90% of total potential energy, they are no longer hungry
            hungry = false;
        }

        if(energy <= 0f || maturity >= 100f)
        {
           ChangeHealth(-0.1f);
        } else if (!hungry){
            ChangeHealth(constitution * 0.001f);
        }

        CheckRecentBirth(); //Check if its baby making time
        Evolve(); //Attempt Evolution

        if (maturity >= matureAge && !hungry && !recentBirth){
            rend.color = Color.magenta;
            lookingForMate = true;
            aggressive = false;
            //Reproduce!
        } else if (maturity >= 10 && energy <= maxEnergy * 0.4f){ //At 50% energy, they get hungry and agressive and will kill for food
            rend.color = Color.red;
            aggressive = true;
        } else {
            lookingForMate = false;
            aggressive = false;
        }
        if ( health <= 0 ){
            RemoveActor();
        }
    }

    void OnCollisionStay2D(Collision2D col){
        GameObject other = col.gameObject;
        // While we collide with an Organic, we want to transfer their organic size into our energy.
        if ((other.CompareTag("Organic") || other.CompareTag("Meat") ) && hungry) // If they collide with an Organic and they have less than 80% of their energy they will eat.
        {
            if(other.CompareTag("Organic")){
                OrganicsBehaviour other_script = other.GetComponent<OrganicsBehaviour>();
                other_script.organicSize -= 5f;
                ChangeEnergy(1f);
            } else if (other.CompareTag("Meat")){
                MeatBehaviour other_script = other.GetComponent<MeatBehaviour>();
                other_script.organicSize -= 15f;
                ChangeEnergy(1f);
            }
        }
        
        if (other.CompareTag("DefaultActor") && aggressive) // If they are feeling agressive, they will attack another actor. Using Energy.
        {
            canAttack = false;
            if (!(lastAttack + attackSpeed < Time.time)) return;
            lastAttack = Time.time;
            canAttack = true;
            ActorBehaviour otherScript = other.GetComponent<ActorBehaviour>();
            if(!otherScript.aggressive){
                ChangeEnergy(-1f);
            } else if (otherScript.aggressive){
                ChangeEnergy(-2f);
            }
            otherScript.health -= attackDamage;
            Instantiate(damageEffect, other.transform.position, other.transform.rotation);
        }
    }

    void  OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.gameObject;
        // If we collide with another Actor
        if (other.CompareTag("DefaultActor") && !aggressive && lookingForMate && !recentBirth) // If they collide with another Actor, produce a child. Reproduction
        {
            ActorBehaviour mateScript = other.GetComponent<ActorBehaviour>();
            if(mateScript.race == race) {
                if(mateScript.maturity < maturity && !mateScript.hungry && !hungry && mateScript.lookingForMate && lookingForMate){
                energy -= maxEnergy / (constitution / 2);
                GameObject new_object = Instantiate(this.gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
                ActorBehaviour newbornScript = new_object.GetComponent<ActorBehaviour>();

                // Newborn Name
                newbornScript.lastName = lastName;

                // Average out everything
                // Because this is done after the newborn is Birthed, it will override the random values assigned at initialization.
                // TODO: average out everything with wieghts to the most extreme.
                newbornScript.strength = SkewedAverage(mateScript.strength, strength);
                newbornScript.dexterity = SkewedAverage(mateScript.dexterity, dexterity);
                newbornScript.constitution = SkewedAverage(mateScript.constitution, constitution);
                newbornScript.wisdom = SkewedAverage(mateScript.wisdom, wisdom);
                newbornScript.intelligence = SkewedAverage(mateScript.intelligence, intelligence);
                newbornScript.charisma = SkewedAverage(mateScript.charisma, charisma);
                lastBirth = Time.time;
                birthCount++;
                recentBirth = true;
                } else if (mateScript.maturity > maturity && !mateScript.hungry && !hungry && mateScript.lookingForMate && lookingForMate){
                energy -= maxEnergy / (constitution / 2);
                lastBirth = Time.time;
                recentBirth = true;
                }
            }
            MoveBackward();
            TurnRight();
        }

        if (other.CompareTag("DefaultActor")){
            ContactPoint2D contact = col.contacts[0];
            ActorBehaviour other_script = other.GetComponent<ActorBehaviour>();
            other_script.rigidBody2D.velocity = -(contact.relativeVelocity) * attackForce * 10f * Time.deltaTime;
        }
    }
    
    void Evolve(){ // If the creature is not capable of giving birth and has atleast 50% energy they may evolve.
        if(!(lastEvolution + evolutionCount * 100f < Time.time)) return;
        if(maturity >= matureAge && recentBirth && energy >= maxEnergy * 0.5f){
        rend.color = Color.blue; //If they are attempting evolution, they are blue
        if(!(lastEvolutionAttempt + 10f < Time.time)) return;
        lastEvolutionAttempt = Time.time;
            if(UnityEngine.Random.Range(0f,100f) < 3f ){
                evolutionCount++;
                lastEvolution = Time.time;
                energy -= maxEnergy * 0.5f;

                //TODO: Change Evolution Mechanic
                // Evolution is currently random increases to random stats. 
                // I would like to make it so that actors can choose which stat to increase on their own.
                float increase = UnityEngine.Random.Range(0.5f,2f);
                int statToIncrease = UnityEngine.Random.Range(1, 6);
                if(statToIncrease == 1) strength += increase;
                else if (statToIncrease == 2) dexterity += increase;
                else if (statToIncrease == 3) constitution += increase;
                else if (statToIncrease == 4) wisdom += increase;
                else if (statToIncrease == 5) intelligence += increase;
                else if (statToIncrease == 6) charisma += increase;
                UpdateActor();
            }
        }
    }

    void CheckRecentBirth(){
        if (!(lastBirth + Mathf.Max(100 * birthCount / (constitution * 0.01f), 100f) < Time.time)) return;
        if( recentBirth ) recentBirth = false;
    }

    public float CompareParentStat(float parent1Stat, float parent2Stat, float average){ // Figures out which of the two stats is the most extreme of the two
        float difference1 = Mathf.Abs(parent1Stat - average);
        float difference2 = Mathf.Abs(parent2Stat - average);
        return difference1 > difference2 ? difference1 : difference2;
    }
    public float SkewedAverage(float parent1Stat, float parent2Stat){ // Skews the average in favour of the extreme value
        float average = (parent1Stat + parent2Stat) / 2;
        return (CompareParentStat(parent1Stat, parent2Stat, average) + parent1Stat + parent2Stat) / 3;
    }

    private void ChangeHealth(float healthChange){
        health += healthChange;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
    private void ChangeEnergy(float energyChange){
        energy += energyChange;
        if(energy > maxEnergy)
        {
            energy = maxEnergy;
        } else if (energy < 0) {
            energy = 0;
        }
    }

    void PreviousFocus(){
        try {
            if (canSeeFocus) // Is there a focus
            {
            int focusMask = focus.layer;
            // Find the Direction to Target
            Vector2 directionToTarget = (focus.transform.position - transform.position).normalized;

            // If in field of view
            if (Vector2.Angle(transform.up, directionToTarget) < fieldOfView / 2) // Is the Focus within the field of view
            {
                float distFocusTarget = Vector2.Distance(focus.transform.position, transform.position);
                if (Physics2D.Raycast(transform.position, directionToTarget, distFocusTarget, obstructionMask)) // Is the focus obstructed?
                {
                    Unfocus();
                }
                if (distFocusTarget > radius + 1f) { // If our focus is beyond our view range. With a little extra to make up for weirdness
                    Unfocus();
                }
            } else {
                
                Unfocus();
            }
            }
        } catch {
            
            Unfocus();
        }
    }

    void FieldOfView(){
        try{
            if(canSeeFocus) PreviousFocus();
            else {
                LayerMask hitLayers = actorMask | defaultMask;
                Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, hitLayers);
                if (rangeCheck.Length > 0){
                float nearestTarget = Mathf.Infinity;
                     // For each in range
                for(int i = 0; i < rangeCheck.Length; i++)
                    {
                        // Is the object its looking at itself? Skip that one.
                        if(rangeCheck[i].gameObject.GetInstanceID() != this.gameObject.GetInstanceID()){
                            // Find the Direction to Target
                            Vector2 directionToTarget = (rangeCheck[i].transform.position - transform.position).normalized;

                            // If in field of view
                            if (Vector2.Angle(transform.up, directionToTarget) < fieldOfView / 2)
                            {
                                //find the nearest target
                                float distContestedTarget = Vector2.Distance(rangeCheck[i].transform.position, transform.position);
                                if( distContestedTarget < nearestTarget){
                                    nearestTarget = distContestedTarget;

                                    // RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distContestedTarget, rangeCheck[i].gameObject.layer);
                                    if (!Physics2D.Raycast(transform.position, directionToTarget, distContestedTarget, obstructionMask))
                                    {
                                        canSeeFocus = true;
                                        focus = rangeCheck[i].gameObject;
                                        i = rangeCheck.Length;
                                    } else {
                                        
                                        Unfocus();
                                    }
                                }
                            } else {
                                
                                Unfocus();
                            }
                        }
                    }
                } else {
                    
                    Unfocus();
                }
            }
        } catch {
            Unfocus();
            
        }
    }

    private void Unfocus(){
        canSeeFocus = false;
        focus = null;
    }

    // Movement Functions
    private void TurnTowards(Transform targetTransform)
    {
        float angle = Vector2.SignedAngle(transform.up, targetTransform.position - transform.position);
        if (angle < 0f)
        {
            TurnRight();
        } else if (angle > 0f) {
            TurnLeft();
        }
    }
    private void TurnAway(Transform targetTransform)
    {
        float angle = Vector2.SignedAngle(transform.up, targetTransform.position - transform.position);
        if (angle < 0f)
        {
            TurnLeft();
        } else if (angle > 0f) {
            TurnRight();
        }
    }
    private void TurnTowardsPosition(Vector3 targetTransform)
    {
        float angle = Vector2.SignedAngle(transform.up, targetTransform - transform.position);
        if (angle < 0f)
        {
            TurnRight();
        } else if (angle > 0f) {
            TurnLeft();
        }
    }

    private void TurnRight(){
        ChangeEnergy(-0.0001f);
        rigidBody2D.rotation -= rotationSpeed;
    }
    private void TurnLeft(){
        ChangeEnergy(-0.0001f);
        rigidBody2D.rotation += rotationSpeed;
    }
    private void MoveForward(){
        ChangeEnergy(-0.0001f);
        transform.position += transform.up * Time.deltaTime * movementSpeed;
    }
    private void MoveBackward(){
        ChangeEnergy(-0.0001f);
        transform.position -= transform.up * Time.deltaTime * movementSpeed;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);

        Vector3 angle01 = DirectionFromAngle(-transform.eulerAngles.z, -fieldOfView / 2);
        Vector3 angle02 = DirectionFromAngle(-transform.eulerAngles.z, fieldOfView / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle01 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle02 * radius);

        if (canSeeFocus){
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, focus.transform.position);
        }
    }

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void RemoveActor()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        //Drop1 should be meat
        for(float i = 0; i <= energy; i++){
            GameObject new_object = Instantiate(drop1, transform.position, transform.rotation);
            float force = UnityEngine.Random.Range(0.1f, 2f) * 0.05f;
            Vector2 randomVector = new Vector2(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f));
            new_object.GetComponent<Rigidbody2D>().velocity = randomVector.normalized * force;
        }
        Destroy(gameObject);
        spawner.SpawnCount -= 1;
    }

}
