using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//////////////////////////////////////////////////////////////////////
//      This class manages settlers and their jobs and tasks
//////////////////////////////////////////////////////////////////////
public class SettlementManager : MonoBehaviour
{
    
    public Settlement settlement;
    public Job[] jobs;


    
    // -------------------------------
    //        Methods
    // -------------------------------

    
      private GameTime lastAssignTime = new GameTime();
    void Start() {
        lastAssignTime.CopyFrom(World.timeNow());
    }

    void Update() {
        // Recalculating Jobs every 30 minutes
        if (World.timeNow() >= (lastAssignTime + new GameTime(0,30,0))) {

            foreach(GameObject pecht_obj in settlement.settlers) {
                Pecht pecht = pecht_obj.GetComponent<Pecht>();
                pecht.startWork();
            }
            
            lastAssignTime.CopyFrom(World.timeNow());

            determineJobPriorities();
            recalculateWorkersNeeded();
            assignJobsToPechts();
        }         
    }

    // Here put all the calculation to determine the Priority
    //   for each job 
    public void determineJobPriorities() {
        foreach(Job job in jobs) {
            job.recalculatePriority(settlement);
        }
    }

    // Perform calculations to understand how many Pechts
    //   we will need for each job
    public void recalculateWorkersNeeded() {
        // For each priority level
        for (int p = 4; p >= 0; p--) {
            bool checkAgain = true;

            // While jobs on current priority level need more workers
            while(checkAgain) {
                checkAgain = false;

                // Check each job in order to check each job if priority == p
                foreach(Job job in jobs) {
 
                    if (job.priority == p) {

                        // If this job needs more workers
                        if ((job.providedWithWorkers < job.workersNeeded()) && (settlement.settlers.Length > countWorkersBusy())) {
                            // then give 1 more and continue to loop
                            job.providedWithWorkers++;
                            checkAgain = true;
                        }
                    }
                }
            }
        }
    }

    // We must already now how much workers we need
    //   Here only needed to decide which Pecht will be better for the jobs
    public void assignJobsToPechts() {
        // For now I am using simple assigment.
        // Later this must choose from most appropriate Pechts
        foreach(Job job in jobs) {
            int given = 0;
            foreach(GameObject pechtObject in settlement.settlers) {
                Pecht pecht = pechtObject.GetComponent<Pecht>();

                // If this Pecht is busy already
                if (pecht.jobName != "") {
                    // Then continue the loop and find another Pecht
                    continue;
                }

                if (given < job.providedWithWorkers) {
                    given ++;
                    pecht.jobName = job.title;
                }
            }
        } 
    }

    public void recalculateEachSettlersStats() {
        // Loop through each Pecht and use recalculateStats()
    }


    private int countWorkersBusy() {
        int total = 0;
        foreach (Job job in jobs) {
            total += job.providedWithWorkers;
        }
        return total;
    }
    
}
