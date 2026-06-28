<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<h3>You have successfully upgraded your account</h3>
<br />
<h5>Download Tawala Designer</h5>
<p>
You will need to download and install a copy of Tawala Designer in order to create your own projects or 
to edit projects downloaded from the Tawala Library. Click the Designer tab above or click the button below: 
</p>
<br />
<a class="roundButton" href="/designer" onclick="this.blur();"><span>Download Tawala Designer</span></a>
<br /><br />
<a href="#" onclick="window.open('/demos/Create your Own Web Apps/Create your Own Web Apps.htm','Video','width=1040,height=700,toolbar=no,menubar=no,status=no,location=no,resizeable=yes,scrollbars=yes'); return false;">
    <span>View the Tawala Designer Demo <img src="/images/template/red-bullet-arrow-right.gif" /></span>
</a>
<br /><br /><br />

<h5>Vist the expanded Library</h5>
<p>
You also have access to an expanded Tawala Library. There you will find many other projects built 
by us and other Tawala designers. Click the Library tab above or the button below.
</p>
<br />
<a class="roundButton" href="/library/search?library=2" onclick="this.blur();"><span>Go to the Tawala Library</span></a>
<br /><br /><br />

<p>
You may switch back to the Intermediate level from this Advanced level at any time by choosing the "Edit Info"
link on the My Tawala page or clicking the button below: 
</p>
<br />
<a class="roundButton" href="/user/account" onclick="this.blur();"><span>Edit my account</span></a>
<br /><br />