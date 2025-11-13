# Feature Specification: Multi-Mouse Input Manager

**Feature Branch**: `001-multi-mouse-input`  
**Created**: 2025-11-13  
**Status**: Draft  
**Input**: User description: "複数のマウス入力を取得するためのマネージャークラス、複数マウス入力動作確認用サンプルシーン、Editor・Build両対応"

## User Scenarios & Testing

### User Story 1 - Initialize Multi-Mouse System (Priority: P1)

A developer imports the Unity6_MultiMouse sample into their project and initializes the multi-mouse input manager in a script. They want to access all connected mice without complex setup or configuration.

**Why this priority**: This is the foundational capability - without a way to initialize and access the system, nothing else works. All other features depend on this.

**Independent Test**: Developer can add the manager to a GameObject, call its initialization method with zero or minimal parameters, and start receiving input events from all connected mice immediately.

**Acceptance Scenarios**:

1. **Given** a new Unity scene with no input setup, **When** a script initializes the MultiMouseInputManager, **Then** the manager detects all currently connected mice and becomes ready to receive input
2. **Given** the manager is initialized, **When** a new mouse is physically connected to the system, **Then** the new mouse is automatically detected and included in subsequent input queries
3. **Given** a mouse is disconnected, **When** input is queried, **Then** that mouse's data is no longer included in the results

---

### User Story 2 - Detect Individual Mouse Button Presses (Priority: P1)

A developer wants to know which physical mouse was pressed and which button (left, right, middle) was pressed, so they can handle input from multiple mice independently.

**Why this priority**: Core requirement - users need to distinguish input from different mice by button press, which is the primary use case.

**Independent Test**: With multiple mice connected, pressing buttons on different mice produces distinct input events that can be correlated to specific mouse devices. Can be tested in both Editor play mode and built application.

**Acceptance Scenarios**:

1. **Given** two mice are connected, **When** left button is pressed on mouse #1, **Then** an event is fired identifying mouse #1 and left button press
2. **Given** two mice are connected, **When** right button is pressed on mouse #2, **Then** an event is fired identifying mouse #2 and right button press
3. **Given** middle button exists, **When** middle button is pressed, **Then** an event is fired identifying the pressed mouse and middle button
4. **Given** a button is held down, **When** another button is pressed on a different mouse, **Then** both button states are tracked independently

---

### User Story 3 - Track Individual Mouse Positions (Priority: P1)

A developer wants to query the current screen-space position of each connected mouse, so they can render cursors or interact with UI elements independently for each mouse.

**Why this priority**: Essential for visual feedback and multi-cursor UI interaction - users need real-time position data for each mouse.

**Independent Test**: By querying mouse positions in a frame, developer can verify that each connected mouse returns accurate screen coordinates. Visual verification in sample scene shows multiple cursors at correct positions.

**Acceptance Scenarios**:

1. **Given** two mice are connected, **When** mouse positions are queried, **Then** each mouse returns its own current screen position independently
2. **Given** mouse #1 moves to position (100, 200), **When** position is queried, **Then** mouse #1's position is (100, 200) and mouse #2's position is unaffected
3. **Given** both mice move simultaneously, **When** positions are queried in the same frame, **Then** both positions reflect current movement independent of each other

---

### User Story 4 - Track Individual Mouse Movement Delta (Priority: P1)

A developer wants to query how much each mouse has moved in the current or previous frame, so they can implement mouse-based camera control or aiming mechanics that work independently per mouse.

**Why this priority**: Essential for interactive applications - users need delta movement for smooth, responsive input (action games require low-latency movement tracking).

**Independent Test**: By querying movement delta in a frame, developer can verify each connected mouse returns movement relative to its last position. Visual verification shows movement magnitude and direction per mouse.

**Acceptance Scenarios**:

1. **Given** two mice are connected, **When** movement deltas are queried in a frame, **Then** each mouse returns its own movement vector (deltaX, deltaY) independent of others
2. **Given** mouse #1 moves 10 pixels right and mouse #2 moves 5 pixels down, **When** deltas are queried, **Then** mouse #1 delta is (10, 0) and mouse #2 delta is (0, 5)
3. **Given** a mouse does not move in a frame, **When** its delta is queried, **Then** the delta is (0, 0)

---

### User Story 5 - Visualize Multi-Mouse Activity in Sample Scene (Priority: P2)

A developer wants to see a clear, interactive visualization of multi-mouse input in action, displaying button states, positions, and movement for all connected mice. This serves both as documentation and as a quick validation tool.

**Why this priority**: High-value reference implementation - shows correct usage patterns and helps users verify their system is working. Can be tested independently without requiring integration into user's own application.

**Independent Test**: Open sample scene in Unity Editor or built application, connect multiple mice, and interact with the scene. All mouse activity (buttons, positions, movement) should update visually in real-time.

**Acceptance Scenarios**:

1. **Given** sample scene is running, **When** a button is pressed on any mouse, **Then** UI or visual element shows which mouse and which button was pressed
2. **Given** sample scene is running, **When** mice move, **Then** visual cursors or indicators move to track each mouse's position independently
3. **Given** sample scene is running, **When** mice move, **Then** movement magnitude or direction indicator updates to show delta movement
4. **Given** sample scene is running in Editor play mode, **When** mice are moved and clicked, **Then** behavior is identical to running in built application

---

### Edge Cases

- What happens when 10+ mice are connected simultaneously?
- How does the system handle rapid button press/release sequences on multiple mice?
- What happens when a mouse is disconnected mid-application execution?
- What happens on systems with no mice connected (should gracefully return empty data)?

## Requirements

### Functional Requirements

- **FR-001**: System MUST detect all Windows mice connected via physical/virtual device drivers
- **FR-002**: System MUST identify each mouse uniquely by device ID or similar persistent identifier
- **FR-003**: System MUST provide access to left, right, and middle mouse button states (pressed, held, released) for each mouse independently
- **FR-004**: System MUST provide current screen-space (pixel) position for each connected mouse
- **FR-005**: System MUST provide movement delta (change in position) for each mouse per frame with minimal latency
- **FR-006**: System MUST implement manager as a C# class accessible via static or singleton pattern with no dependency on external libraries
- **FR-007**: System MUST work identically in Unity Editor play mode and in built Windows applications
- **FR-008**: System MUST use only C# DllImport calls to Windows API (user32.dll or equivalent), with no C++ wrappers or COM interop
- **FR-009**: System MUST provide a sample scene that visualizes input from all connected mice (button presses, cursor positions, movement)
- **FR-010**: Sample scene MUST be launchable from a fresh Unity 6 editor without additional build steps or configuration

### Non-Functional Requirements

- **NFR-001**: Input latency MUST be negligible (< 1 frame at 60 FPS) to support action game use cases
- **NFR-002**: No external assets, plugins, or libraries outside of Unity 6 standard modules
- **NFR-003**: Code MUST be simple and understandable for beginners - no advanced design patterns, no abstraction layers without clear justification
- **NFR-004**: API surface MUST be minimal - main method for querying mice should require 0-1 parameters
- **NFR-005**: All DllImport declarations MUST include Windows API documentation references and parameter explanations
- **NFR-006**: System MUST support up to at least 4 mice simultaneously without performance degradation

### Key Entities

- **Mouse Device**: Represents a single connected mouse with unique ID, current position, button states, and movement delta
- **Mouse Input State**: Snapshot of a single mouse's state at a point in time (position, buttons, delta)
- **Input Manager**: Central manager class that aggregates input from all connected mice and exposes query methods

## Success Criteria

### Measurable Outcomes

- **SC-001**: All 4 connected mice are detected and tracked simultaneously in the sample scene without data loss or cross-contamination
- **SC-002**: Button press events are delivered with zero frame delay (observed within the same frame the input occurred)
- **SC-003**: Movement delta values show <1ms latency between physical mouse movement and queryable data
- **SC-004**: Sample scene runs at 60+ FPS on standard Windows development machine with 4 mice connected
- **SC-005**: A complete integration example (code + sample scene) is provided that a developer can copy/paste and use immediately
- **SC-006**: First-time user can understand and verify multi-mouse functionality within 5 minutes of opening the project

