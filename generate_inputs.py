import pyaml
import yaml
import collections

defaults = yaml.safe_load('''
- serializedVersion: 3
  m_Name: Horizontal
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: left
  positiveButton: right
  altNegativeButton: a
  altPositiveButton: d
  gravity: 3
  dead: 0.001
  sensitivity: 3
  snap: 1
  invert: 0
  type: 0
  axis: 0
  joyNum: 0
- serializedVersion: 3
  m_Name: Vertical
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: down
  positiveButton: up
  altNegativeButton: s
  altPositiveButton: w
  gravity: 3
  dead: 0.001
  sensitivity: 3
  snap: 1
  invert: 0
  type: 0
  axis: 0
  joyNum: 0
- serializedVersion: 3
  m_Name: Mouse X
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: 
  altNegativeButton: 
  altPositiveButton: 
  gravity: 0
  dead: 0
  sensitivity: 0.1
  snap: 0
  invert: 0
  type: 1
  axis: 0
  joyNum: 0
- serializedVersion: 3
  m_Name: Mouse Y
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: 
  altNegativeButton: 
  altPositiveButton: 
  gravity: 0
  dead: 0
  sensitivity: 0.1
  snap: 0
  invert: 0
  type: 1
  axis: 1
  joyNum: 0
- serializedVersion: 3
  m_Name: Mouse ScrollWheel
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: 
  altNegativeButton: 
  altPositiveButton: 
  gravity: 0
  dead: 0
  sensitivity: 0.1
  snap: 0
  invert: 0
  type: 1
  axis: 2
  joyNum: 0
- serializedVersion: 3
  m_Name: Horizontal
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: 
  altNegativeButton: 
  altPositiveButton: 
  gravity: 0
  dead: 0.19
  sensitivity: 1
  snap: 0
  invert: 0
  type: 2
  axis: 0
  joyNum: 0
- serializedVersion: 3
  m_Name: Vertical
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: 
  altNegativeButton: 
  altPositiveButton: 
  gravity: 0
  dead: 0.19
  sensitivity: 1
  snap: 0
  invert: 1
  type: 2
  axis: 1
  joyNum: 0
- serializedVersion: 3
  m_Name: Submit
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: return
  altNegativeButton: 
  altPositiveButton: joystick button 0
  gravity: 1000
  dead: 0.001
  sensitivity: 1000
  snap: 0
  invert: 0
  type: 0
  axis: 0
  joyNum: 0
- serializedVersion: 3
  m_Name: Submit
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: enter
  altNegativeButton: 
  altPositiveButton: space
  gravity: 1000
  dead: 0.001
  sensitivity: 1000
  snap: 0
  invert: 0
  type: 0
  axis: 0
  joyNum: 0
- serializedVersion: 3
  m_Name: Cancel
  descriptiveName: 
  descriptiveNegativeName: 
  negativeButton: 
  positiveButton: escape
  altNegativeButton: 
  altPositiveButton: joystick button 1
  gravity: 1000
  dead: 0.001
  sensitivity: 1000
  snap: 0
  invert: 0
  type: 0
  axis: 0
  joyNum: 0
''')

axes = []
axes.extend(defaults)

keys = {
    1: ('w', 's', 'a', 'd', 'space', 'shift'),
    2: ('i', 'k', 'j', 'l', 'u', 'o'),
    3: (None, None, None, None, None, None),
    4: (None, None, None, None, None, None),
}

control_key_template = {
    "serializedVersion": 3,
    "m_Name": "",
    "descriptiveName": None,
    "descriptiveNegativeName": None,
    "negativeButton": "",
    "positiveButton": "",
    "altNegativeButton": "",
    "altPositiveButton": "",
    "gravity": 3,
    "dead": 0.001,
    "sensitivity": 3,
    "snap": 1,
    "invert": 0,
    "type": 0,
    "axis": 0,
    "joyNum": 0
}

action_key_template = dict(control_key_template, **{
    "gravity": 1000,
    "dead": 0.001,
    "sensitivity": 1000,
    "snap": 0,
    "invert": 0,
    "type": 0,
    "axis": 0,
    "joyNum": 0,
})

joy_axis_template = dict(control_key_template, **{
    "gravity": 0,
    "dead": 0.19,
    "sensitivity": 1,
    "snap": 0,
    "invert": 0,
    "type": 2,
    "axis": 0,
    "joyNum": 0
})

for player_num in range(1, 5):
    up, down, left, right, shoot, alt = keys[player_num]
    if left:
        axes.append(dict(control_key_template,
            m_Name=f"X{player_num}",
            descriptiveName=f"P{player_num} Horizontal Key",
            negativeButton=left,
            positiveButton=right,
        ))
    if up:
        axes.append(dict(control_key_template,
            m_Name=f"Y{player_num}",
            descriptiveName=f"P{player_num} Vertical Key",
            negativeButton=down,
            positiveButton=up,
        ))
    if shoot:
        axes.append(dict(action_key_template,
            m_Name=f"Shoot{player_num}",
            descriptiveName=f"P{player_num} Shoot Key",
            positiveButton=shoot,
        ))
    if alt:
        axes.append(dict(action_key_template,
            m_Name=f"Alt{player_num}",
            descriptiveName=f"P{player_num} Alt Key",
            positiveButton=alt,
        ))

    axes.append(dict(joy_axis_template,
        m_Name=f"X{player_num}",
        descriptiveName=f"P{player_num} Horizontal Analog",
        joyNum=player_num,
    ))    

    axes.append(dict(joy_axis_template,
        m_Name=f"Y{player_num}",
        descriptiveName=f"P{player_num} Vertical Analog",
        joyNum=player_num,
        axis=1,
    ))

    axes.append(dict(action_key_template,
        m_Name=f"Shoot{player_num}",
        descriptiveName=f"P{player_num} Shoot Button",
        positiveButton="joystick button 0",
    ))

    axes.append(dict(action_key_template,
        m_Name=f"Alt{player_num}",
        descriptiveName=f"P{player_num} Alt Button",
        positiveButton="joystick button 1",
    ))

input_manager_obj = collections.OrderedDict((
  ("m_ObjectHideFlags", 0),
  ("serializedVersion", 2),
  ("m_Axes", axes),
))

print('''
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!13 &1
'''.strip())
print(pyaml.dump({'InputManager': input_manager_obj}))