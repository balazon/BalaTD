using UnityEngine;
using System.Collections;

//The heart of decoupling the gui from the model (Observer pattern):
// 1) model elements have an instance of MyObservable in them (MyObservable observable)
// 2) gui elements subscribe to a model element like this:
//     element.observable.notify += methodName;
// 3) when the model element's state changes, it calls notifyObservers:
//     observable.notifyObservers();
//    notifyObservers will call each subscribed gui element's method they subscribed with (methodName)
public class MyObservable {

	public delegate void MyVoidDelegate();
	
	public MyVoidDelegate notify;
	
	public void notifyObservers()
	{
		if(notify != null)
		{
			notify();
		}
	}
}
