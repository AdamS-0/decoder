# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [0.4.1] 2026-09-30

### Fixed
- enums for 256+
- replacing with newer definitions


## [0.4.0] 2026-09-29

### Fixed
- array size processing

### Added
- parser for basic maths operations
- support for bitshifts, XOR, AND, and others bitwise operators in enums and defines


## [0.3.1] 2026-09-27

### Added
- removing macro instructions


## [0.3.0] 2026-09-26

### Changed
- Renamed ListExtension.cs
- Removed option "Rebuild pattern"

### Added
- red bold highlighting 0 bits fields
- clearing all comments from input source code
- error messages during parsing


## [0.2.1] 2026-09-24

### Fixed
- bitsize with concated property name and ':'

### Added
- collapsing larger than 2 embedded levels structures and arrays with no element > 10
- {struct/enum/union} identifiers to field types
- cleanout of block comments from main source code
- recalculation of used structs


## [0.2.0] 2026-09-19

### Fixed
- enums with given values
- selection of patterns

### Changed
- Refactor of processing arrays, part 2
- Modification of DecoderGui
- Cleaning raw data on input from LeftMenu
- Refactor of ParseParamsOfAtox

### Added
- hiding of LeftMenu
- remove instance from "_ownTypes"


## [0.1.5] 2026-09-12

### Fixed
- printing enum's
- bugs with closing DecoderGui

### Changed
- Optimized highlighting algorithm

### Added
- enums parsing
- decoding of unions
- support for one line macros
- processing of the comments


## [0.1.4] 2026-09-06

### Changed
- Redefined information about DecoderField, now dObject

### Added
- block parsing
- union example & class diagram
- redirection to console when started from one


## [0.1.3] 2026-08-30

### Changed
- Simplified DecoderField stored data

### Added
- new pattern form
- more context menus
- highlighting corresponding nibbles of selected field
- btn "Rebuild current pattern"


## [0.1.2] 2026-08-27

### Changed
- Refactored generating TreeDetailView for structures to common form

### Added
- basic notes + small code cleaning
- better formatting to Value & Raw in TreeDetailView
- inherited cloning for sub structures
- inherited structs to one level


## [0.1.1] 2026-08-23

### Added
- icon
- MainListener for shortcut LShift+RButton (opens left side conetxt menu)


## [0.1.0] 2026-08-21

### Added
- First commit of Decoder
