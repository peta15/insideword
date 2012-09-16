<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <%= Html.Title("Terms") %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="info terms">
        <h1>Terms of Use</h1>
        <div class="h1hr"></div>
        <br />
        <i>Updated as of June 13, 2010</i>
        <ol>
            <li>Your Acceptance</li>
            <ol>
                <li>By using and/or visiting this website (collectively, including all content available
                    through the <i>InsideWord</i>.com domain name and any country-specific domains or
                    subdomains thereof, the "<i>InsideWord</i> Website", or "Website"), you signify
                    your agreement to (1) these terms and conditions (the "Terms of Use"), and (2) <i>InsideWord</i>'s
                    privacy notice, found at <%: Html.ActionLink(Url.ActionAbsolute(MVC.Info.Privacy()), MVC.Info.Privacy()) %>
                    and incorporated here by reference. If you do not agree to any of these terms and
                    the <i>InsideWord</i> privacy notice do not use the <i>InsideWord</i> Website.</li>
                <p>
                    Although we may attempt to notify you when major changes are made to these Terms
                    of Use, you should periodically review the most up-to-date version (<%: Html.ActionLink(Url.ActionAbsolute(MVC.Info.Terms()), MVC.Info.Terms())%>).
                    <i>InsideWord</i> may, in its sole discretion, modify or revise these Terms of Use
                    and policies at any time, and you agree to be bound by such modifications or revisions.
                    Nothing in this Agreement shall be deemed to confer any third-party rights or benefits.</p>
            </ol>
            <li><i>InsideWord</i> Website</li>
            <ol>
                <li>These Terms of Use apply to all users of the <i>InsideWord</i> Website, including
                    users who are also contributors of article content, comments, information, and other
                    materials or services on the Website. </li>
                <li>The <i>InsideWord</i> Website may contain links to third party websites that are
                    not owned or controlled by <i>InsideWord</i>. <i>InsideWord</i> has no control over,
                    and assumes no responsibility for, the content, privacy policies, or practices of
                    any third party websites. In addition, <i>InsideWord</i> will not and cannot censor
                    or edit the content of any third-party site. By using the Website, you expressly
                    relieve <i>InsideWord</i> from any and all liability arising from your use of any
                    third-party website.</li>
                <li>Accordingly, we encourage you to be aware when you leave the <i>InsideWord</i> Website
                    and to read the terms and conditions and privacy policy of each other website that
                    you visit.</li>
            </ol>
            <li><i>InsideWord</i> Accounts</li>
            <ol>
                <li>In order to access some features of the Website, you will have to create an <i>InsideWord</i>
                    account. You may never use another's account without permission. When creating your
                    account, you must provide accurate and complete information. You are solely responsible
                    for the activity that occurs on your account, and you must keep your account password
                    secure. You must notify <i>InsideWord</i> immediately of any breach of security
                    or unauthorized use of your account.</li>
                <li><i>InsideWord</i> will not be liable for your losses caused by any unauthorized
                    use of your account.</li>
            </ol>
            <li>General Use of the Website—Permissions and Restrictions</li>
            <ol>
                <p>
                    <i>InsideWord</i> hereby grants you permission to access and use the Website as
                    set forth in these Terms of Use, provided that:</p>
                <li>You agree not to distribute in any medium any part of the Website, including but
                    not limited to User Submissions (defined below), without <i>InsideWord</i>'s prior
                    written authorization.</li>
                <li>You agree not to alter or modify any part of the Website.</li>
                <li>You agree not to use or launch any automated system, including without limitation,
                    "robots," "spiders," or "offline readers," that accesses the Website in a manner
                    that sends more request messages to the <i>InsideWord</i> servers in a given period
                    of time than a human can reasonably produce in the same period by using a conventional
                    on-line web browser. Notwithstanding the foregoing, <i>InsideWord</i> grants the
                    operators of public search engines permission to use spiders to copy materials from
                    the site for the sole purpose of and solely to the extent necessary for creating
                    publicly available searchable indices of the materials, but not caches or archives
                    of such materials. <i>InsideWord</i> reserves the right to revoke these exceptions
                    either generally or in specific cases. You agree not to collect or harvest any personally
                    identifiable information, including account names, from the Website, nor to use
                    the communication systems provided by the Website (e.g. comments, email) for any
                    commercial solicitation purposes. You agree not to solicit, for commercial purposes,
                    any users of the Website with respect to their User Submissions.</li>
                <li>In your use of the Website, you will otherwise comply with the terms and conditions
                    of these Terms of Use and all applicable local, national, and international laws
                    and regulations.</li>
                <li><i>InsideWord</i> reserves the right to discontinue any aspect of the <i>InsideWord</i>
                    Website at any time.</li>
            </ol>
            <li>Your Use of Content on the Site</li>
            <ol>
                <p>
                    In addition to the general restrictions above, the following restrictions and conditions
                    apply specifically to your use of content on the <i>InsideWord</i> Website.</p>
                <li>The content on the <i>InsideWord</i> Website, except all User Submissions (as defined
                    below), including without limitation, the text, software, scripts, graphics, photos,
                    sounds, music, videos, interactive features and the like ("Content") and the trademarks,
                    service marks and logos contained therein ("Marks"), are owned by or licensed to
                    <i>InsideWord</i>, subject to copyright and other intellectual property rights under
                    the law. Content on the Website is provided to you AS IS for your information and
                    personal use only and may not be downloaded, copied, modified, produced, reproduced,
                    distributed, transmitted, broadcast, displayed, sold, licensed, translated, published,
                    performed or otherwise exploited for any other purposes whatsoever without the prior
                    written consent of the respective owners. <i>InsideWord</i> reserves all rights
                    not expressly granted in and to the Website and the Content.</li>
                <li>You may access User Submissions solely:</li>
                <ul>
                    <li>for your information and personal use;
                        <li>as intended through the normal functionality of the <i>InsideWord</i>
                    Service; and
                </ul>
                <li>User Comments are made available to you for your information and personal use solely
                    as intended through the normal functionality of the <i>InsideWord</i> Service. User
                    Comments are made available "as is", and may not be used, copied, modified, produced,
                    reproduced, distributed, transmitted, broadcast, displayed, sold, licensed, downloaded,
                    translated, published, performed or otherwise exploited in any manner not intended
                    by the normal functionality of the <i>InsideWord</i> Service or otherwise as prohibited
                    under this Agreement.</li>
                <li>You may access User Submissions and other content only as permitted under this Agreement.
                    <i>InsideWord</i> reserves all rights not expressly granted in and to the <i>InsideWord</i>
                    Content and the <i>InsideWord</i> Service.</li>
                <li>You agree to not engage in the use, copying, or distribution of any of the Content
                    other than expressly permitted herein, including any use, copying, or distribution
                    of User Submissions of third parties obtained through the Website for any commercial
                    purposes.</li>
                <li>You agree not to circumvent, disable or otherwise interfere with security-related
                    features of the <i>InsideWord</i> Website or features that prevent or restrict use
                    or copying of any Content or enforce limitations on use of the <i>InsideWord</i>
                    Website or the Content therein.</li>
                <li>You understand that when using the <i>InsideWord</i> Website, you will be exposed
                    to User Submissions from a variety of sources, and that <i>InsideWord</i> is not
                    responsible for the accuracy, usefulness, safety, or intellectual property rights
                    of or relating to such User Submissions. You further understand and acknowledge
                    that you may be exposed to User Submissions that are inaccurate, offensive, indecent,
                    or objectionable, and you agree to waive, and hereby do waive, any legal or equitable
                    rights or remedies you have or may have against <i>InsideWord</i> with respect thereto,
                    and agree to indemnify and hold <i>InsideWord</i>, its Owners/Operators, affiliates,
                    and/or licensors, harmless to the fullest extent allowed by law regarding all matters
                    related to your use of the site.</li>
            </ol>
            <li>Your User Submissions and Conduct</li>
            <ol>
                <li>As an <i>InsideWord</i> account holder you may submit article content ("User Articles")
                    and comment content ("User Comments"). User Articles and User Comments are collectively
                    referred to as "User Submissions." You understand that whether or not such User
                    Submissions are published, <i>InsideWord</i> does not guarantee any confidentiality
                    with respect to any User Submissions.</li>
                <li>You shall be solely responsible for your own User Submissions and the consequences
                    of posting or publishing them. In connection with User Submissions, you affirm,
                    represent, and/or warrant that: you own or have the necessary licenses, rights,
                    consents, and permissions to use and authorize <i>InsideWord</i> to use all patent,
                    trademark, trade secret, copyright or other proprietary rights in and to any and
                    all User Submissions and have all necessary consents to collect, use and disclose
                    any personally identifiable information contained or displayed in any and all User
                    Submissions to enable inclusion and use of the User Submissions in the manner contemplated
                    by the Website and these Terms of Use.</li>
                <li>For clarity, you retain all of your ownership rights in your User Submissions. However,
                    by submitting User Submissions to <i>InsideWord</i>, you hereby grant <i>InsideWord</i>
                    a worldwide, non-exclusive, royalty-free, sublicenseable and transferable license
                    to use, reproduce, distribute, prepare derivative works of, display, and perform
                    the User Submissions in connection with the <i>InsideWord</i> Website and <i>InsideWord</i>'s
                    (and its successors' and affiliates') business, including without limitation for
                    promoting and redistributing part or all of the <i>InsideWord</i> Website (and derivative
                    works thereof) in any media formats and through any media channels. You also hereby
                    waive any moral rights you may have in your User Submissions and grant each user
                    of the <i>InsideWord</i> Website a non-exclusive license to access your User Submissions
                    through the Website, and to use, reproduce, distribute, display and perform such
                    User Submissions as permitted through the functionality of the Website and under
                    these Terms of Use. The above licenses granted by you in User Articles terminate
                    within a commercially reasonable time after you remove or delete your User Submissions
                    from the <i>InsideWord</i> Service. You understand and agree, however, that <i>InsideWord</i>
                    may retain, but not display, distribute, or perform, server copies of User Submissions
                    that have been removed or deleted. The above licenses granted by you in User Comments
                    are perpetual and irrevocable.</li>
                <li>In connection with User Submissions, you further agree that you will not submit
                    material that is copyrighted, protected by trade secret or otherwise subject to
                    third party proprietary rights, including privacy and publicity rights, unless you
                    are the owner of such rights or have permission from their rightful owner and the
                    necessary consents from any individuals whose personally identifiable information
                    is contained in such material to post the material and to grant <i>InsideWord</i>
                    all of the license rights granted herein.</li>
                <li>You further agree that you will not, in connection with User Submissions, submit
                    material that is contrary to the <i>InsideWord</i> Community Guidelines, found at
                    <%: Html.ActionLink(Url.ActionAbsolute(MVC.Info.Guidelines()), MVC.Info.Guidelines())%>,
                    which may be updated from time to time, or contrary to applicable local, national,
                    and international laws and regulations.</li>
                <li><i>InsideWord</i> does not endorse any User Submission or any opinion, recommendation,
                    or advice expressed therein, and <i>InsideWord</i> expressly disclaims any and all
                    liability in connection with User Submissions. <i>InsideWord</i> does not permit
                    copyright infringing activities and infringement of intellectual property rights
                    on its Website, and <i>InsideWord</i> will remove within a reasonable period of
                    time all Content and User Submissions if properly notified that such Content or
                    User Submission infringes on another's intellectual property rights or contravenes
                    any applicable privacy legislation. <i>InsideWord</i> reserves the right to remove
                    Content and User Submissions without prior notice.</li>
            </ol>
            <li>Account Termination Policy</li>
            <ol>
                <li><i>InsideWord</i> will terminate a User's access to its Website if, under appropriate
                    circumstances, the User is determined to be a repeat infringer.</li>
                <li><i>InsideWord</i> reserves the right to decide whether Content or a User Submission
                    is appropriate and complies with these Terms of Use for violations other than copyright
                    infringement or privacy law, such as, but not limited to, hate crimes, pornography,
                    obscene or defamatory material, or excessive length. <i>InsideWord</i> may remove
                    such User Submissions and/or terminate a User's access for uploading such material
                    in violation of these Terms of Use at any time, without prior notice and at its
                    sole discretion.</li>
            </ol>
            <li>Copyright Policy</li>
            <ol>
                <li><i>InsideWord</i> operates a copyright policy in relation to any User Submissions
                    that are alleged to infringe the copyright of a third party. </li>
                <li>As part of <i>InsideWord</i>'s copyright policy, <i>InsideWord</i> will terminate
                    user access to the Website if a user has been determined to be a repeat infringer.
                </li>
            </ol>
            <li>Warranty and Condition Disclaimer</li>
            <p>
                YOU AGREE THAT YOUR USE OF THE <i>InsideWord</i> WEBSITE SHALL BE AT YOUR SOLE RISK.
                TO THE FULLEST EXTENT PERMITTED BY LAW, <i>InsideWord</i>, ITS OFFICERS, DIRECTORS,
                EMPLOYEES, AND AGENTS DISCLAIM ALL WARRANTIES AND CONDITIONS, WHETHER WRITTEN, ORAL,
                EXPRESS, IMPLIED, LEGAL, STATUTORY, CONTRACTUAL, EXTRA-CONTRACTUAL, DELICTUAL OR
                IN TORT, AND WHETHER ARISING BY LAW, STATUTE, USAGE OF TRADE, CUSTOM, COURSE OF
                DEALING OR PERFORMANCE, OR THE PARTIES' CONDUCT OR COMMUNICATION WITH ONE ANOTHER,
                OR AS A RESULT OF THE NATURE OF THESE TERMS AND CONDITIONS, THE WEBSITE AND YOUR
                USE THEREOF OR IN CONFORMITY WITH USAGE, EQUITY OR LAW OR OTHERWISE. <i>InsideWord</i>
                MAKES NO WARRANTIES, CONDITIONS OR REPRESENTATIONS ABOUT THE ACCURACY OR COMPLETENESS
                OF THIS SITE'S CONTENT OR THE CONTENT OF ANY SITES LINKED TO THIS SITE AND ASSUMES
                NO LIABILITY OR RESPONSIBILITY FOR ANY (I) ERRORS, MISTAKES, OR INACCURACIES OF
                CONTENT, (II) PERSONAL INJURY, PROPERTY, BODILY, MORAL OR MATERIAL DAMAGE OF ANY
                NATURE WHATSOEVER, RESULTING FROM YOUR ACCESS TO AND USE OF OUR WEBSITE, (III) ANY
                UNAUTHORIZED ACCESS TO OR USE OF OUR SECURE SERVERS AND/OR ANY AND ALL PERSONAL
                INFORMATION AND/OR FINANCIAL INFORMATION STORED THEREIN, (IV) ANY INTERRUPTION OR
                CESSATION OF TRANSMISSION TO OR FROM OUR WEBSITE, (IV) ANY BUGS, VIRUSES, TROJAN
                HORSES, OR THE LIKE WHICH MAY BE TRANSMITTED TO OR THROUGH OUR WEBSITE BY ANY THIRD
                PARTY, AND/OR (V) ANY ERRORS OR OMISSIONS IN ANY CONTENT OR FOR ANY LOSS OR DAMAGE
                OF ANY KIND INCURRED AS A RESULT OF THE USE OF ANY CONTENT POSTED, EMAILED, TRANSMITTED,
                OR OTHERWISE MADE AVAILABLE VIA THE <i>InsideWord</i> WEBSITE. <i>InsideWord</i>
                EXPRESSLY DISCLAIMS ANY WARRANTY OR CONDITION OF ANY KIND OF FITNESS FOR A PARTICULAR
                OR GENERAL PURPOSE, QUALITY, MERCHANTABILITY, WORKMANSHIP, NON-INFRINGEMENT, OR
                TITLE AND OWNERSHIP. <i>InsideWord</i> DOES NOT WARRANT, ENDORSE, GUARANTEE, OR
                ASSUME RESPONSIBILITY FOR ANY PRODUCT OR SERVICE ADVERTISED OR OFFERED BY A THIRD
                PARTY THROUGH THE <i>InsideWord</i> WEBSITE OR ANY HYPERLINKED WEBSITE OR FEATURED
                IN ANY BANNER OR OTHER ADVERTISING, AND <i>InsideWord</i> WILL NOT BE A PARTY TO
                OR IN ANY WAY BE RESPONSIBLE FOR MONITORING ANY TRANSACTION BETWEEN YOU AND THIRD-PARTY
                PROVIDERS OF PRODUCTS OR SERVICES. AS WITH THE PURCHASE OF A PRODUCT OR SERVICE
                THROUGH ANY MEDIUM OR IN ANY ENVIRONMENT, YOU SHOULD USE YOUR BEST JUDGMENT AND
                EXERCISE CAUTION WHERE APPROPRIATE. TO THE MAXIMUM EXTENT PERMITTED BY LAW, THE
                PROVISIONS OF THE UNITED NATIONS CONVENTION ON CONTRACTS FOR THE INTERNATIONAL SALE
                OF GOODS ARE HEREBY DISCLAIMED.
            </p>
            <li>Limitation of Liability</li>
            <p>
                IN NO EVENT SHALL <i>InsideWord</i>, ITS OFFICERS, DIRECTORS, EMPLOYEES, OR AGENTS,
                BE LIABLE TO YOU FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, PUNITIVE, OR CONSEQUENTIAL
                DAMAGES WHATSOEVER RESULTING FROM ANY (I) ERRORS, MISTAKES, OR INACCURACIES OF CONTENT,
                (II) PERSONAL INJURY, PROPERTY, BODILY, MORAL OR MATERIAL DAMAGE OF ANY NATURE WHATSOEVER,
                RESULTING FROM YOUR ACCESS TO AND USE OF OUR WEBSITE, (III) ANY UNAUTHORIZED ACCESS
                TO OR USE OF OUR SECURE SERVERS AND/OR ANY AND ALL PERSONAL INFORMATION AND/OR FINANCIAL
                INFORMATION STORED THEREIN, (IV) ANY INTERRUPTION OR CESSATION OF TRANSMISSION TO
                OR FROM OUR WEBSITE, (IV) ANY BUGS, VIRUSES, TROJAN HORSES, OR THE LIKE, WHICH MAY
                BE TRANSMITTED TO OR THROUGH OUR WEBSITE BY ANY THIRD PARTY, AND/OR (V) ANY ERRORS
                OR OMISSIONS IN ANY CONTENT OR FOR ANY LOSS OR DAMAGE OF ANY KIND INCURRED AS A
                RESULT OF YOUR USE OF ANY CONTENT POSTED, EMAILED, TRANSMITTED, OR OTHERWISE MADE
                AVAILABLE VIA THE <i>InsideWord</i> WEBSITE, WHETHER BASED ON WARRANTY, CONTRACT,
                TORT, OR ANY OTHER LEGAL THEORY, AND WHETHER ARISING BY LAW, STATUTE, USAGE OF TRADE,
                CUSTOM, COURSE OF DEALING OR PERFORMANCE, OR THE PARTIES' CONDUCT OR COMMUNICATION
                WITH ONE ANOTHER, OR AS A RESULT OF THE NATURE OF THESE TERMS AND CONDITIONS OR
                IN CONFORMITY WITH USAGE, EQUITY OR LAW OR OTHERWISE, AND WHETHER OR NOT THE COMPANY
                IS ADVISED OF THE POSSIBILITY OF SUCH DAMAGES. THE FOREGOING LIMITATION OF LIABILITY
                SHALL APPLY TO THE FULLEST EXTENT PERMITTED BY LAW IN THE APPLICABLE JURISDICTION.
                YOU SPECIFICALLY ACKNOWLEDGE THAT <i>InsideWord</i> SHALL NOT BE LIABLE FOR USER
                SUBMISSIONS OR THE DEFAMATORY, OFFENSIVE, OR ILLEGAL CONDUCT OF ANY THIRD PARTY
                AND THAT THE RISK OF HARM OR DAMAGE FROM THE FOREGOING RESTS ENTIRELY WITH YOU.
                The Website is controlled and offered by <i>InsideWord</i> from its facilities in
                the United States of America. <i>InsideWord</i> makes no representations that the
                <i>InsideWord</i> Website is appropriate or available for use in other locations.
                Those who access or use the <i>InsideWord</i> Website from other jurisdictions do
                so at their own volition and are responsible for compliance with local law.
            </p>
            <li>Ability to Accept Terms of Use</li>
            <p>
                You affirm that you are either more than the age of majority in your jurisdiction
                of residence, or an emancipated minor, or possess legal parental or guardian consent,
                and are fully able and competent to enter into the terms, conditions, obligations,
                affirmations, representations, and warranties set forth in these Terms of Use, and
                to abide by and comply with these Terms of Use. In any case, you affirm that you
                are over the age of 13, as the <i>InsideWord</i> Website is not intended for children
                under 13. If you are under 13 years of age, then please do not use the <i>InsideWord</i>
                Website—there are lots of other great web sites for you. Talk to your parents about
                what sites are appropriate for you.
            </p>
            <li>Assignment</li>
            <p>
                These Terms of Use, and any rights and licenses granted hereunder, may not be transferred
                or assigned by you, but may be assigned by <i>InsideWord</i> without restriction.
            </p>
            <li>General</li>
            <p>
                You agree that: (i) the <i>InsideWord</i> Website shall be deemed solely based in
                New Jersey; and (ii) the <i>InsideWord</i> Website shall be deemed a passive website
                that does not give rise to personal jurisdiction over <i>InsideWord</i>, either
                specific or general, in jurisdictions other than New Jersey. Unless prohibited by
                local law, these Terms of Use shall be governed by the internal substantive laws
                of the State of New Jersey, without respect to its conflict of laws principles.
                Unless prohibited by local law, any claim or dispute between you and <i>InsideWord</i>
                that arises in whole or in part from the <i>InsideWord</i> Website shall be decided
                exclusively by a court of competent jurisdiction located in New Jersey. These Terms
                of Use, together with the Privacy Notice at <%: Html.ActionLink( Url.ActionAbsolute(MVC.Info.Privacy()), MVC.Info.Privacy())%>
                and any other legal notices published
                by <i>InsideWord</i> on the Website, shall constitute the entire agreement between
                you and <i>InsideWord</i> concerning the <i>InsideWord</i> Website. If any provision
                of these Terms of Use is deemed invalid by a court of competent jurisdiction, the
                invalidity of such provision shall not affect the validity of the remaining provisions
                of these Terms of Use, which shall remain in full force and effect. No waiver of
                any term of this these Terms of Use shall be deemed a further or continuing waiver
                of such term or any other term, and <i>InsideWord</i>'s failure to assert any right
                or provision under these Terms of Use shall not constitute a waiver of such right
                or provision. <i>InsideWord</i> reserves the right to amend these Terms of Use at
                any time and without notice, and it is your responsibility to review these Terms
                of Use for any changes. Your use of the <i>InsideWord</i> Website following any
                amendment of these Terms of Use will signify your assent to and acceptance of its
                revised terms. TO THE MAXIMUM EXTENT PERMITTED BY LOCAL LAW, YOU AND <i>InsideWord</i>
                AGREE THAT ANY CAUSE OF ACTION ARISING OUT OF OR RELATED TO THE <i>InsideWord</i>
                WEBSITE MUST COMMENCE WITHIN ONE (1) YEAR AFTER THE CAUSE OF ACTION ACCRUES. OTHERWISE,
                SUCH CAUSE OF ACTION IS PERMANENTLY BARRED.
            </p>
            <li>Language</li>
            <p>
                The parties confirm that it is their wish that this agreement, as well as any other
                documents relating to this agreement, including notices, schedules and authorizations
                have been and shall be drawn up in the English language only. Les signataires confirment
                leur volonté que la présente convention, de même que tous les documents s'y rattachant,
                y compris tout avis, annexe et autorisation, soient rédigés en anglais seulement.
            </p>
    </div>
</asp:Content>
