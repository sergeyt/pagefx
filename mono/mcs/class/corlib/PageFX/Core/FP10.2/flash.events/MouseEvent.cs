using System;
using System.Runtime.CompilerServices;

namespace flash.events
{
    /// <summary>
    /// An object dispatches a MouseEvent object into the event flow whenever mouse events occur.
    /// A mouse event is usually generated by a user input device, such as a mouse or a trackball,
    /// that uses a pointer.
    /// </summary>
    [PageFX.AbcInstance(309)]
    [PageFX.ABC]
    [PageFX.FP9]
    public class MouseEvent : flash.events.Event
    {
        /// <summary>
        /// Defines the value of the type property of a click event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; there is no default behavior to cancel.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(0)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String CLICK;

        /// <summary>
        /// Defines the value of the type property of a doubleClick event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; there is no default behavior to cancel.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(1)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String DOUBLE_CLICK;

        /// <summary>
        /// Defines the value of the type property of a mouseDown event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; the default behavior cannot be canceled.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(2)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String MOUSE_DOWN;

        /// <summary>
        /// Defines the value of the type property of a mouseMove event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; the default behavior cannot be canceled.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(3)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String MOUSE_MOVE;

        /// <summary>
        /// Defines the value of the type property of a mouseOut event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; the default behavior cannot be canceled.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.relatedObjectThe display list object to which the pointing device now points.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(4)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String MOUSE_OUT;

        /// <summary>
        /// Defines the value of the type property of a mouseOver event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; the default behavior cannot be canceled.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.relatedObjectThe display list object to which the pointing device was pointing.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(5)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String MOUSE_OVER;

        /// <summary>
        /// Defines the value of the type property of a mouseUp event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; the default behavior cannot be canceled.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(6)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String MOUSE_UP;

        /// <summary>
        /// Defines the value of the type property of a mouseWheel event object.
        /// This event has the following properties:PropertyValuebubblestruebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; the default behavior cannot be canceled.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.deltaThe number of lines that that each notch on the mouse wheel represents.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(7)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String MOUSE_WHEEL;

        /// <summary>
        /// Defines the value of the type property of a rollOut event object.
        /// This event has the following properties:PropertyValuebubblesfalsebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; there is no default behavior to cancel.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.relatedObjectThe display list object to which the pointing device now points.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(8)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String ROLL_OUT;

        /// <summary>
        /// Defines the value of the type property of a rollOver event object.
        /// This event has the following properties:PropertyValuebubblesfalsebuttonDowntrue if the primary mouse button is pressed; false otherwise.cancelablefalse; there is no default behavior to cancel.ctrlKeytrue if the Control key is active; false if it is inactive.currentTargetThe object that is actively processing the Event
        /// object with an event listener.relatedObjectThe display list object to which the pointing device was pointing.localXThe horizontal coordinate at which the event occurred relative to the containing sprite.localYThe vertical coordinate at which the event occurred relative to the containing sprite.shiftKeytrue if the Shift key is active; false if it is inactive.stageXThe horizontal coordinate at which the event occurred in global stage coordinates.stageYThe vertical coordinate at which the event occurred in global stage coordinates.targetThe InteractiveObject instance under the pointing device.
        /// The target is not always the object in the display list
        /// that registered the event listener. Use the currentTarget
        /// property to access the object in the display list that is currently processing the event.
        /// </summary>
        [PageFX.AbcClassTrait(9)]
        [PageFX.ABC]
        [PageFX.FP9]
        public static Avm.String ROLL_OVER;

        /// <summary>The horizontal coordinate at which the event occurred relative to the containing sprite.</summary>
        public extern virtual double localX
        {
            [PageFX.AbcInstanceTrait(9)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(10)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>The vertical coordinate at which the event occurred relative to the containing sprite.</summary>
        public extern virtual double localY
        {
            [PageFX.AbcInstanceTrait(11)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(12)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>A reference to a display list object that is related to the event. For example, when a mouseOut event occurs, relatedObject represents the display list object to which the pointing device now points. This property applies only to the mouseOut and mouseOver events.</summary>
        public extern virtual flash.display.InteractiveObject relatedObject
        {
            [PageFX.AbcInstanceTrait(13)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(14)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Indicates whether the Control key is active (true) or inactive (false).
        /// On Macintosh computers, you must use this property to represent the Command key.
        /// </summary>
        public extern virtual bool ctrlKey
        {
            [PageFX.AbcInstanceTrait(15)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(16)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>Reserved for future use.</summary>
        public extern virtual bool altKey
        {
            [PageFX.AbcInstanceTrait(17)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(18)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Indicates whether the Shift key is active (true) or inactive
        /// (false).
        /// </summary>
        public extern virtual bool shiftKey
        {
            [PageFX.AbcInstanceTrait(19)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(20)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>Indicates whether the primary mouse button is pressed (true) or not (false).</summary>
        public extern virtual bool buttonDown
        {
            [PageFX.AbcInstanceTrait(21)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(22)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// Indicates how many lines should be scrolled for each unit the user rotates the
        /// mouse wheel. A positive delta value indicates an upward scroll; a negative
        /// value indicates a downward scroll. Typical values are 1 to 3, but faster
        /// rotation may produce larger values. This setting depends on the device
        /// and operating system and is usually configurable by the user. This
        /// property applies only to the MouseEvent.mouseWheel event.
        /// </summary>
        public extern virtual int delta
        {
            [PageFX.AbcInstanceTrait(23)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(24)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        /// <summary>
        /// The horizontal coordinate at which the event occurred in global Stage coordinates.
        /// This property is calculated when the localX property is set.
        /// </summary>
        public extern virtual double stageX
        {
            [PageFX.AbcInstanceTrait(25)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        /// <summary>
        /// The vertical coordinate at which the event occurred in global Stage coordinates.
        /// This property is calculated when the localY property is set.
        /// </summary>
        public extern virtual double stageY
        {
            [PageFX.AbcInstanceTrait(26)]
            [PageFX.ABC]
            [PageFX.FP9]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        public extern virtual bool isRelatedObjectInaccessible
        {
            [PageFX.AbcInstanceTrait(30)]
            [PageFX.ABC]
            [PageFX.FP("10.2")]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [PageFX.AbcInstanceTrait(31)]
            [PageFX.ABC]
            [PageFX.FP("10.2")]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY, flash.display.InteractiveObject relatedObject, bool ctrlKey, bool altKey, bool shiftKey, bool buttonDown, int delta);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY, flash.display.InteractiveObject relatedObject, bool ctrlKey, bool altKey, bool shiftKey, bool buttonDown);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY, flash.display.InteractiveObject relatedObject, bool ctrlKey, bool altKey, bool shiftKey);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY, flash.display.InteractiveObject relatedObject, bool ctrlKey, bool altKey);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY, flash.display.InteractiveObject relatedObject, bool ctrlKey);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY, flash.display.InteractiveObject relatedObject);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX, double localY);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable, double localX);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles, bool cancelable);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type, bool bubbles);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MouseEvent(Avm.String type);

        [PageFX.AbcInstanceTrait(7)]
        [PageFX.ABC]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern override flash.events.Event clone();

        [PageFX.AbcInstanceTrait(8)]
        [PageFX.ABC]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern override Avm.String toString();

        /// <summary>Instructs Flash Player to render after processing of this event completes, if the display list has been modified.</summary>
        [PageFX.AbcInstanceTrait(27)]
        [PageFX.ABC]
        [PageFX.FP9]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern virtual void updateAfterEvent();


    }
}