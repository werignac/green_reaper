using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

[RequireComponent(typeof(Collider2D))]
public class PlantSlower : MonoBehaviour
{
    private HashSet<GameObject> contacts;

    //[SerializeField]
    private static float distInFront = 0.1f;
    //[SerializeField]
    private static float distInBack = 0.25f;

    private DepthOrganizer organizer;

    [SerializeField]
    private Dimmer dimmer;

    private void Start()
    {
        contacts = new HashSet<GameObject>();
        organizer = transform.parent.GetComponent<DepthOrganizer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.gameObject.GetComponent<PlayerController>();
        DepthOrganizer playerOrganizer = other.gameObject.GetComponent<DepthOrganizer>();


        if (controller != null)
        {
            float playerHeight = playerOrganizer.GetOrigin().y;
            float heightDiff =  playerHeight - organizer.GetOrigin().y;

            dimmer?.SetDim(heightDiff > 0);            

            if (heightDiff < distInBack && heightDiff > -distInFront)
                EnterPlant(controller);
            else
                ExitPlant(controller.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Collider2D collider = collision.gameObject.GetComponent<Collider2D>();
        if (collider != null)
            OnTriggerEnter2D(collider);    
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController controller = other.gameObject.GetComponent<PlayerController>();

        if (controller != null)
        {
            ExitPlant(controller.gameObject);
        }
    }

    private void EnterPlant(PlayerController toEnter)
    {
        if (!contacts.Contains(toEnter.gameObject))
        {
            bool added = toEnter.BuffMaxSpeed(new PlantSpeedDecrease(toEnter.gameObject, this));
            added = added && toEnter.BuffMaxVelocityChange(new PlantSpeedDecrease(toEnter.gameObject, this));

            if (added)
                EnterPlant(toEnter.gameObject);
        }
    }

    private void EnterPlant(GameObject toEnter)
    {
        if (!contacts.Contains(toEnter))
            contacts.Add(toEnter);
    }

    private void ExitPlant(GameObject toExit)
    {
        if (contacts.Contains(toExit))
        {
            contacts.Remove(toExit.gameObject);
            dimmer?.SetDim(false);
        }
    }

    public bool InContact(GameObject g)
    {
        return contacts.Contains(g);
    }

    private class PlantSpeedDecrease : Buff<float>
    {
        public string Name => "Plant Speed Decrease";

        private HashSet<PlantSlower> inContacts;

        private GameObject target;

        public PlantSpeedDecrease(GameObject _target, PlantSlower initalContact)
        {
            target = _target;
            inContacts = new HashSet<PlantSlower>();
            inContacts.Add(initalContact);
        }

        public float Affect(float value)
        {
            return value * 0.25f;
        }

        public void Combine(Buff<float> other)
        {
            PlantSpeedDecrease otherCast = other as PlantSpeedDecrease;

            foreach (PlantSlower plant in otherCast.inContacts)
            {
                inContacts.Add(plant);
            }
        }

        public BuffType GetBuffType()
        {
            return BuffType.DEBUFF;
        }

        public bool IsActive()
        {
            bool anyContacts = false;

            HashSet<PlantSlower> toRemove = new HashSet<PlantSlower>();

            foreach (PlantSlower plant in inContacts)
            {
                if (plant == null)
                {
                    toRemove.Add(plant);
                }
                else
                {
                    bool isInContact = plant.InContact(target);
                    anyContacts = anyContacts || isInContact;
                }
            }

            foreach (PlantSlower removal in toRemove)
            {
                inContacts.Remove(removal);
            }

            return anyContacts;
        }

        public void Wipe() { }
    }
}
