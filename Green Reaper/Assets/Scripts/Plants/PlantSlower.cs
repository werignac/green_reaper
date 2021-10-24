using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

[RequireComponent(typeof(Collider2D))]
public class PlantSlower : MonoBehaviour
{
    private HashSet<GameObject> contacts;

    private void Start()
    {
        contacts = new HashSet<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.gameObject.GetComponent<PlayerController>();

        if (controller != null)
        {
            contacts.Add(controller.gameObject);
            controller.BuffMaxSpeed(new PlantSpeedDecrease(controller.gameObject, this));
            controller.BuffMaxVelocityChange(new PlantSpeedDecrease(controller.gameObject, this));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController controller = other.gameObject.GetComponent<PlayerController>();

        if (controller != null)
        {
            contacts.Remove(controller.gameObject);
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
            return BuffType.NATURAL;
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
