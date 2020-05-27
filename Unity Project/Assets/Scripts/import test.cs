using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class importtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

namespace testingFYP
{
    public class test1 : MonoBehaviour
    {
        public int test1int = 3;

        void Start()
        {
            test1int = 3;
            Debug.Log("Test1 start: " + test1int.ToString());
        }

        public int getNumber()
        {
            return 7;
        }
    }

    public class test2 : MonoBehaviour
    {
        public int test2int = 5;

        void Start()
        {
            test2int = 5;
            Debug.Log("Test2 start: " + test2int.ToString());
        }
    }
}
