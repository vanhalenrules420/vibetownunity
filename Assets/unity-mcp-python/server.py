from mcp.server.fastmcp import FastMCP, Context, Image
import logging
from dataclasses import dataclass
from contextlib import asynccontextmanager
from typing import AsyncIterator, Dict, Any, List
from config import config
from tools import register_all_tools
from unity_connection import get_unity_connection, UnityConnection

# Configure logging using settings from config
logging.basicConfig(
    level=getattr(logging, config.log_level),
    format=config.log_format
)
logger = logging.getLogger("UnityMCPServer")

# Global connection state
_unity_connection: UnityConnection = None

@asynccontextmanager
async def server_lifespan(server: FastMCP) -> AsyncIterator[Dict[str, Any]]:
    """Handle server startup and shutdown."""
    global _unity_connection
    logger.info("UnityMCP server starting up")
    try:
        _unity_connection = get_unity_connection()
        logger.info("Connected to Unity on startup")
    except Exception as e:
        logger.warning(f"Could not connect to Unity on startup: {str(e)}")
        _unity_connection = None
    try:
        yield {}
    finally:
        if _unity_connection:
            _unity_connection.disconnect()
            _unity_connection = None
        logger.info("UnityMCP server shut down")

# Initialize MCP server
mcp = FastMCP(
    "UnityMCP",
    description="Unity Editor integration via Model Context Protocol",
    lifespan=server_lifespan
)

# Register all tools
register_all_tools(mcp)

# Asset Creation Strategy

@mcp.prompt()
def asset_creation_strategy() -> str:
    """Guide for creating and managing assets in Unity."""
    return (
        "Unity MCP Server Tools and Best Practices:\n\n"
        "1. **Editor Control**\n"
        "   - `editor_action` - Performs editor-wide actions such as `PLAY`, `PAUSE`, `STOP`, `BUILD`, `SAVE`\n"
        "2. **Scene Management**\n"
        "   - `get_current_scene()`, `get_scene_list()` - Get scene details\n"
        "   - `open_scene(path)`, `save_scene(path)` - Open/save scenes\n"
        "   - `new_scene(path)`, `change_scene(path, save_current)` - Create/switch scenes\n\n"
        "3. **Object Management**\n"
        "   - ALWAYS use `find_objects_by_name(name)` to check if an object exists before creating or modifying it\n"
        "   - `create_object(name, type)` - Create objects (e.g. `CUBE`, `SPHERE`, `EMPTY`, `CAMERA`)\n"
        "   - `delete_object(name)` - Remove objects\n"
        "   - `set_object_transform(name, location, rotation, scale)` - Modify object position, rotation, and scale\n"
        "   - `add_component(name, component_type)` - Add components to objects (e.g. `Rigidbody`, `BoxCollider`)\n"
        "   - `remove_component(name, component_type)` - Remove components from objects\n"
        "   - `get_object_properties(name)` - Get object properties\n"
        "   - `find_objects_by_name(name)` - Find objects by name\n"
        "   - `get_hierarchy()` - Get object hierarchy\n"
        "4. **Script Management**\n"
        "   - ALWAYS use `list_scripts(folder_path)` or `view_script(path)` to check if a script exists before creating or updating it\n"
        "   - `create_script(name, type, namespace, template)` - Create scripts\n"
        "   - `view_script(path)`, `update_script(path, content)` - View/modify scripts\n"
        "   - `attach_script(object_name, script_name)` - Add scripts to objects\n"
        "   - `list_scripts(folder_path)` - List scripts in folder\n\n"
        "5. **Asset Management**\n"
        "   - ALWAYS use `get_asset_list(type, search_pattern, folder)` to check if an asset exists before creating or importing it\n"
        "   - `import_asset(source_path, target_path)` - Import external assets\n"
        "   - `instantiate_prefab(path, pos_x, pos_y, pos_z, rot_x, rot_y, rot_z)` - Create prefab instances\n"
        "   - `create_prefab(object_name, path)`, `apply_prefab(object_name, path)` - Manage prefabs\n"
        "   - `get_asset_list(type, search_pattern, folder)` - List project assets\n"
        "   - Use relative paths for Unity assets (e.g., 'Assets/Models/MyModel.fbx')\n"
        "   - Use absolute paths for external files\n\n"
        "6. **Material Management**\n"
        "   - ALWAYS check if a material exists before creating or modifying it\n"
        "   - `set_material(object_name, material_name, color)` - Apply/create materials\n"
        "   - Use RGB colors (0.0-1.0 range)\n\n"
        "7. **Best Practices**\n"
        "   - ALWAYS verify existence before creating or updating any objects, scripts, assets, or materials\n"
        "   - Use meaningful names for objects and scripts\n"
        "   - Keep scripts organized in folders with namespaces\n"
        "   - Verify changes after modifications\n"
        "   - Save scenes before major changes\n"
        "   - Use full component names (e.g., 'Rigidbody', 'BoxCollider')\n"
        "   - Provide correct value types for properties\n"
        "   - Keep prefabs in dedicated folders\n"
        "   - Regularly apply prefab changes\n"
    )

# Run the server
if __name__ == "__main__":
    mcp.run(transport='stdio')