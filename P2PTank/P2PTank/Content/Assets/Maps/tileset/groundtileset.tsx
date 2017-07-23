<?xml version="1.0" encoding="UTF-8"?>
<tileset name="land" tilewidth="32" tileheight="32" spacing="1" tilecount="49" columns="7">
 <image source="tilesland.png" width="256" height="256"/>
 <terraintypes>
  <terrain name="Grass" tile="8"/>
  <terrain name="Dirt" tile="29"/>
  <terrain name="Wall" tile="11"/>
 </terraintypes>
 <tile id="0" terrain="1,1,1,0"/>
 <tile id="1" terrain="1,1,0,0"/>
 <tile id="2" terrain="1,1,0,1"/>
 <tile id="3" terrain=",,,2">
  <objectgroup draworder="index">
   <object id="3" name="o00" x="18" y="32">
    <polyline points="0,0 14,-14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="4" terrain=",,2,2">
  <objectgroup draworder="index">
   <object id="1" name="o10" x="0" y="18">
    <polyline points="0,0 32,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="5" terrain=",,2,">
  <objectgroup draworder="index">
   <object id="1" name="020" x="0" y="18">
    <polyline points="0,0 14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="6" terrain=",,,2">
  <objectgroup draworder="index">
   <object id="1" name="o00a" x="32" y="18">
    <polyline points="0,0 -14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="7" terrain="1,0,1,0"/>
 <tile id="8" terrain="0,0,0,0"/>
 <tile id="9" terrain="0,1,0,1"/>
 <tile id="10" terrain=",2,,2">
  <objectgroup draworder="index">
   <object id="1" name="o01" x="18" y="0">
    <polyline points="0,0 0,32"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="11" terrain="2,2,2,2"/>
 <tile id="12" terrain="2,,2,">
  <objectgroup draworder="index">
   <object id="1" name="o21" x="14" y="0">
    <polyline points="0,0 0,32"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="13" terrain=",2,,">
  <objectgroup draworder="index">
   <object id="1" name="o01" x="18" y="0">
    <polyline points="0,0 14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="14" terrain="1,0,1,1"/>
 <tile id="15" terrain="0,0,1,1"/>
 <tile id="16" terrain="0,1,1,1"/>
 <tile id="17" terrain=",2,,">
  <objectgroup draworder="index">
   <object id="1" name="o02" x="18" y="0">
    <polyline points="0,0 14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="18" terrain="2,2,,">
  <objectgroup draworder="index">
   <object id="1" name="o12" x="0" y="14">
    <polyline points="0,0 32,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="19" terrain="2,,,">
  <objectgroup draworder="index">
   <object id="1" name="o22" x="14" y="0">
    <polyline points="0,0 -14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="20" terrain=",,2,">
  <objectgroup draworder="index">
   <object id="1" name="o10a" x="0" y="18">
    <polyline points="0,0 14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="21" terrain="0,0,0,1"/>
 <tile id="22" terrain="0,0,1,1"/>
 <tile id="23" terrain="0,0,1,0"/>
 <tile id="24" terrain="2,2,2,">
  <objectgroup draworder="index">
   <object id="1" name="i00" x="32" y="14">
    <polyline points="0,0 -6,0 -18,12 -18,18"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="25" terrain="2,2,,">
  <objectgroup draworder="index">
   <object id="1" name="i10" x="32" y="14">
    <polyline points="0,0 -32,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="26" terrain="2,2,,2">
  <objectgroup draworder="index">
   <object id="1" name="i20" x="18" y="32">
    <polyline points="0,0 0,-6 -12,-18 -18,-18"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="27" terrain="2,,,">
  <objectgroup draworder="index">
   <object id="1" name="o11a" x="14" y="0">
    <polyline points="0,0 -14,14"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="28" terrain="0,1,0,1"/>
 <tile id="29" terrain="1,1,1,1"/>
 <tile id="30" terrain="1,0,1,0"/>
 <tile id="31" terrain="2,,2,">
  <objectgroup draworder="index">
   <object id="1" name="i01" x="14" y="0">
    <polyline points="0,0 0,32"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="33" terrain=",2,,2">
  <objectgroup draworder="index">
   <object id="1" name="i21" x="18" y="0">
    <polyline points="0,0 0,32"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="34" terrain="0,0,0,1"/>
 <tile id="35" terrain="0,1,0,0"/>
 <tile id="36" terrain="1,1,0,0"/>
 <tile id="37" terrain="1,0,0,0"/>
 <tile id="38" terrain="2,,2,2">
  <objectgroup draworder="index">
   <object id="1" name="i02" x="14" y="0">
    <polyline points="0,0 0,6 11,18 18,18"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="39" terrain=",,2,2">
  <objectgroup draworder="index">
   <object id="1" name="i12" x="0" y="18">
    <polyline points="0,0 32,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="40" terrain=",2,2,2">
  <objectgroup draworder="index">
   <object id="1" name="i22" x="0" y="18">
    <polyline points="0,0 6,0 18,-12 18,-18"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="41" terrain="0,1,0,0"/>
 <tile id="42" terrain="1,1,1,0"/>
 <tile id="43" terrain="1,1,0,1"/>
 <tile id="44" terrain="1,0,1,1"/>
 <tile id="45" terrain="0,1,1,1"/>
 <tile id="47" terrain="1,0,0,0"/>
 <tile id="48" terrain="0,0,1,0"/>
</tileset>
