# SkillSets

## Sets of skills provided with permissions/commands

This plugins allow you to create several sets of skills for your arena or RP server.
You can assign one or more SkillSets to PermissionGroups.

If a player never ran the command `/skillsets skillsetname` and is eligible to two or more default SkillSets, the plugins will
select the first most skilled SkillSet.

If a player want to have a specific skillset, he can list them through the command **/skillsets**
and select one of them with the command **/skillsets skillsetname** (skillsetname is the &lt;Name&gt; defined in the configuration file)

### Commands

*   **/skillsets** show the command usage and skill list relative to your permissions
*   **/skillsets yourskillsetname** set your skills according to the configuration

### Permissions

```xml
<Permission Cooldown="0">skillsets</Permission>
<Permission Cooldown="0">skillsets.police</Permission>
<Permission Cooldown="0">skillsets.yourskillsetname</Permission>
```

### &lt;SkillSetsConfiguration&gt;

<table>

<thead>

<tr>

<td>
<strong>Xml tag name</strong>
</td>

<td><strong>Default value</strong></td>

<td><strong>Description</strong></td>

</tr>

</thead>

<tbody>

<tr>

<td>StorageType</td>

<td>file</td>

<td><strong>file</strong>: to save skillsets assignation on the disk<br><strong>memory</strong>: to save them into the memory (when the server is shutdown, nothing is saved)</td>

</tr>

<tr>

<td>FilePath</td>

<td>EnabledSkillSets.xml</td>

<td>The path where the file is located (only StorageType: file)</td>

</tr>

<tr>

<td>PeriodicSave</td>

<td>10000</td>

<td>The delay between saves in milliseconds (only StorageType: file (for now))</td>

</tr>

</tbody>

</table>

### &lt;SkillSet&gt;

<table>

<thead>

<tr>

<td>
<strong>Xml tag name</strong>
</td>

<td><strong>Example Value</strong></td>

<td><strong>Description</strong></td>

</tr>

</thead>

<tbody>

<tr>

<td>Name</td>

<td>offense</td>

<td>The name of the skillset</td>

</tr>

<tr>

<td>Default</td>

<td>true</td>

<td>Define which SkillSet will be automatically assigned to player (you can set all your SkillSet/Default to true or false)</td>

</tr>

<tr>

<td>Skills</td>

<td>

```xml
<Skill Name="Agriculture" Level="7" />
<Skill Name="Engineer" Level="3" />
```

</td>

<td>The skill list to be updated. All skills not referenced in this list will be Leveled to zero by default.</td>

</tr>

</tbody>

</table>

### Example

#### All skills maxed for people with vip permission group (eg for an arena)

##### Rocket/Plugins/SkillSets/SkillSets.configuration.xml

In this example, you dont need to give the 'skillsets' permission. The command will never be used by your players because all skillsets have &lt;Default&gt;true&lt;/Default&gt;.

The &lt;Default&gt;true&lt;/Default&gt; of skillsets.maxed will assign automatically to the player all skills on connection or after death.

```xml
<?xml version="1.0" encoding="utf-8"?>
<SkillSetsConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <StorageType>file</StorageType>
  <FilePath>EnabledSkillSets.xml</FilePath>
  <PeriodicSave>10000</PeriodicSave>
  <SkillSets>
    <SkillSet>
      <Name>maxed</Name>
      <Default>true</Default>
      <Skills>
        <Skill Name="Agriculture" Level="7" />
        <Skill Name="Cardio" Level="5" />
        <Skill Name="Cooking" Level="3" />
        <Skill Name="Crafting" Level="3" />
        <Skill Name="Dexerity" Level="5" />
        <Skill Name="Diving" Level="5" />
        <Skill Name="Engineer" Level="3" />
        <Skill Name="Exercise" Level="5" />
        <Skill Name="Fishing" Level="5" />
        <Skill Name="Healing" Level="7" />
        <Skill Name="Immunity" Level="5" />
        <Skill Name="Mechanic" Level="5" />
        <Skill Name="Outdoors" Level="5" />
        <Skill Name="Overkill" Level="7" />
        <Skill Name="Parkour" Level="5" />
        <Skill Name="Sharpshooter" Level="7" />
        <Skill Name="Sneakybeaky" Level="7" />
        <Skill Name="Strength" Level="5" />
        <Skill Name="Survival" Level="5" />
        <Skill Name="Toughness" Level="5" />
        <Skill Name="Vitality" Level="5" />
        <Skill Name="Warmblooded" Level="5" />
      </Skills>
    </SkillSet>
  </SkillSets>
</SkillSetsConfiguration>
```

##### Rocket/Permission.config.xml

```xml
<RocketPermissions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <DefaultGroup>default</DefaultGroup>
  <Groups>
    <Group>
      <Id>default</Id>
      <DisplayName>Guest</DisplayName>
      <Prefix />
      <Suffix />
      <Color>white</Color>
      <Members />
      <Priority>100</Priority>
      <Permissions>
        <Permission Cooldown="0">p</Permission>
      </Permissions>
    </Group>
    <Group>
      <Id>vip</Id>
      <DisplayName>VIP</DisplayName>
      <Prefix />
      <Suffix />
      <Color>FF9900</Color>
      <Members />
      <ParentGroup>default</ParentGroup>
      <Priority>100</Priority>
      <Permissions>
        <Permission Cooldown="0">skillsets.maxed</Permission>
      </Permissions>
    </Group>
  </Groups>
</RocketPermissions>
```

### TODO

*   Handle Cooldown for skills.<SkillSetName>
*   Absolute <FilePath> Permit to set the FilePath to a shared location